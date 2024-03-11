using DobirnaGraServer.Game.State;
using DobirnaGraServer.Hubs;
using DobirnaGraServer.Models.GameRPC;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public sealed class Lobby : IDisposable, IAsyncDisposable
	{
		public enum UserAction
		{
			Joined,
			Leaved
		}

		public delegate void EventUserChanged(Lobby lobby, UserProfile profile, UserAction action);

		public event EventUserChanged? OnUserChanged;

		public Guid Id { get; init; }

		private readonly IHubContext<GameHub, IGameClient> _hubContext;

		public string Name { get; init; }

		public InviteCode InviteCode { get; init; }

		private readonly List<UserProfile> _users = [];
		public IReadOnlyList<UserProfile> Users => _users;

		private readonly List<PlayerPlace> _places;
		public IReadOnlyList<PlayerPlace> Places => _places;

		private UserProfile? Master { get; set; }

		private StateMachine _gameStateMachine = new();

		public Lobby(IHubContext<GameHub, IGameClient> hubContext, string name, int initialNumberPlaces)
		{
			_hubContext = hubContext;
			Name = name;
			Id = Guid.NewGuid();
			InviteCode = new InviteCode(Id);

			_places = Enumerable.Range(0, initialNumberPlaces).Select(e => new PlayerPlace()).ToList();

			_gameStateMachine.DefineState(new IdleGameState{ OwnerLobby = this });
			_gameStateMachine.DefineState(new RoundGameState{ OwnerLobby = this });
			_gameStateMachine.SetInitState<IdleGameState>();
			_gameStateMachine.DefineTransition<IdleGameState, RoundGameState>((state, transitAction) =>
			{
				state.OnStartGame += transitAction;
			});
		}

		~Lobby()
		{
			Dispose();
		}

		public void Dispose()
		{
			DisposeAsync().GetAwaiter().GetResult();
		}

		public async ValueTask DisposeAsync()
		{
			InviteCode.Dispose();
			await KickAllUsersAsync();
		}

		public bool HasUser(UserProfile user)
		{
			return _users.Any(e => e == user);
		}

		private bool FindSeat(UserProfile user, out PlayerPlace? table)
		{
			var res = _places.FirstOrDefault(table => table.User == user);
			table = res;
			return res != null;
		}

		public void SetNumberPlaces(UserProfile caller, int number)
		{
			if (caller != Master)
				throw new InvalidOperationException("There is no permission to change the score!");

			if (_places.Capacity < number)
			{
				_places.Capacity = number;
			}

			if (number < _places.Count)
			{
				_places.Sort((l, r) =>
				{
					if (l.User == null && r.User != null)
						return 1;
					if (l.User != null && r.User == null)
						return -1;
					return 0;
				});
				_places.RemoveRange(number, _places.Count - number);
			}
			else if (number > _places.Count)
			{
				_places.AddRange(Enumerable.Range(0, number - _places.Count).Select(e => new PlayerPlace()));
			}

			NotifyLobbyChangedAsync();
		}

		public void RemovePlace(UserProfile caller, int index)
		{
			if (caller != Master)
				throw new InvalidOperationException("There is no permission to change the score!");

			if (index >= _places.Count)
				throw new InvalidOperationException("There's no such seat.");

			_places.RemoveAt(index);

			NotifyLobbyChangedAsync();
		}

		public void ChangeScore(UserProfile caller, int placeIndex, int newScore)
		{
			if (caller != Master)
				throw new InvalidOperationException("There is no permission to change the score!");

			if (placeIndex >= _places.Count)
				throw new InvalidOperationException("There's no such seat.");

			_places[placeIndex].Score = newScore;

			NotifyLobbyChangedAsync();
		}

		public void SeatMaster(UserProfile user)
		{
			if (!HasUser(user))
				throw new InvalidOperationException("This user is not in the lobby");

			if (Master != null)
				throw new InvalidOperationException("The master's seat is already taken!");

			UnseatImpl(user, true);

			Master = user;

			NotifyLobbyChangedAsync();
		}

		public void Seat(UserProfile user, int index)
		{
			if (!HasUser(user))
				throw new InvalidOperationException("This user is not in the lobby");

			if(index >= _places.Count)
				throw new InvalidOperationException("There's no such seat.");

			if (_places[index].User != null)
				throw new InvalidOperationException("The seat is already taken!");

			UnseatImpl(user, true);

			_places[index].User = user;

			NotifyLobbyChangedAsync();
		}

		private void UnseatImpl(UserProfile user, bool groupSilent)
		{
			bool changed = false;

			if (Master == user)
			{
				Master = null;
				changed = true;
			}

			if (FindSeat(user, out PlayerPlace? table) && table != null)
			{
				table.User = null;
				changed = true;
			}

			if (!groupSilent && changed)
				NotifyLobbyChangedAsync();
		}

		public void Unseat(UserProfile user)
		{
			UnseatImpl(user, false);
		}

		public void Interact(UserProfile caller)
		{
			if (_gameStateMachine.CurrentState is IdleGameState idleState)
			{
				if (caller == Master)
				{
					idleState.StartGame();
				}
				else
				{
					idleState.ToggleReady(caller);
				}
			}
		}

		public async Task JoinUserAsMasterAsync(UserProfile user, CancellationToken ct)
		{
			if (Master != null)
				throw new InvalidOperationException("The master's seat is already taken!");

			try
			{
				Master = user;
				await JoinUserAsync(user, ct);
			}
			catch (Exception)
			{
				Master = null;
				throw;
			}
		}

		public async Task JoinUserAsync(UserProfile user, CancellationToken ct)
		{
			if (user.CurrentLobby is {} currentLobby)
				throw new InvalidOperationException($"the user is already in lobby: {currentLobby.Name} ({currentLobby.Id})");

			_users.Add(user);
			user.CurrentLobby = this;

			await _hubContext.Clients.Client(user.ConnectionId)
				.OnLobbyChanged(LobbyAction.Joined, ConvertToRpcData());

			if (_gameStateMachine.CurrentState != null)
			{
				await _hubContext.Clients.Client(user.ConnectionId)
					.OnGameStateChanged(GameStateAction.Entered, _gameStateMachine.CurrentState.GetStateData());
			}

			NotifyLobbyChangedAsync();

			await _hubContext.Groups.AddToGroupAsync(user.ConnectionId, Id.ToString(), ct);
			user.OnProfileChanged += NotifyLobbyChangedAsync;

			OnUserChanged?.Invoke(this, user, UserAction.Joined);
		}

		private async Task LeaveUserExtAsync(UserProfile user, bool groupSilent, CancellationToken ct)
		{
			UnseatImpl(user, true);

			_users.Remove(user);
			user.CurrentLobby = null;

			await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString(), ct);
			user.OnProfileChanged -= NotifyLobbyChangedAsync;

			await _hubContext.Clients.Client(user.ConnectionId)
				.OnLobbyChanged(LobbyAction.Leaved, null);

			if(!groupSilent)
				NotifyLobbyChangedAsync();

			OnUserChanged?.Invoke(this, user, UserAction.Leaved);
		}

		public Task LeaveUserAsync(UserProfile user, CancellationToken ct)
		{
			return LeaveUserExtAsync(user, false, ct);
		}

		public async Task KickAllUsersAsync(CancellationToken ct = default)
		{
			if (_users.Count == 0)
				return;

			var users = _users.ToArray();

			var tasks = users.Select((u) => LeaveUserExtAsync(u, true, ct));

			await Task.WhenAll(tasks);
		}

		private LobbyData ConvertToRpcData()
		{
			return new LobbyData
			{
				Id = Id,
				Name = Name,
				InviteCode = InviteCode,
				Places = Places.Select(p => p.ConvertToRpcData()).ToList(),
				Master = new MasterData
				{
					UserId = Master?.Id,
					UserName = Master?.Name,
					IsOccupied = Master != null,
					ImageId = $"{Master?.Avatar?.Id}",
				}
			};
		}

		private async void NotifyLobbyChangedAsync()
		{
			await _hubContext.Clients
				.Group(Id.ToString())
				.OnLobbyChanged(LobbyAction.Updated, ConvertToRpcData())
				.ConfigureAwait(false);
		}

		public async void NotifyGameStateChangedAsync(BaseStateData? info, bool isNewState)
		{
			await _hubContext.Clients
				.Group(Id.ToString())
				.OnGameStateChanged(isNewState ? GameStateAction.Entered : GameStateAction.Updated, info)
				.ConfigureAwait(false);
		}
	}
}

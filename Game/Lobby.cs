using DobirnaGraServer.Hubs;
using DobirnaGraServer.Models.MessageTypes;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public sealed class Lobby : ILobby, IDisposable, IAsyncDisposable
	{
		public delegate void EventNumberUserChanged(Lobby lobby);

		public event EventNumberUserChanged? OnNumberUserChanged;

		public Guid Id { get; init; }

		private readonly IHubContext<GameHub, IGameClient> _hubContext;

		public string Name { get; init; }

		public InviteCode InviteCode { get; init; }

		public IEnumerable<IProfile> Users => UserList;

		public IEnumerable<IPlace> Places => PlacesList;

		private List<UserProfile> UserList { get; init; } = [];

		private List<PlayerPlace> PlacesList { get; init; } = [];

		public IProfile? Master { get; private set; } = null;

		public Lobby(IHubContext<GameHub, IGameClient> hubContext, string name)
		{
			_hubContext = hubContext;
			Name = name;

			Id = Guid.NewGuid();
			InviteCode = new InviteCode(Id);
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
			return UserList.Any(e => e == user);
		}

		private bool FindSeat(UserProfile user, out PlayerPlace? table)
		{
			var res = PlacesList.FirstOrDefault(table => table.User == user);
			table = res;
			return res != null;
		}

		public void SetNumberSeats(int number, bool groupSilent)
		{
			PlacesList.Capacity = number;
			if (number < PlacesList.Count)
				PlacesList.RemoveRange(number, PlacesList.Count - number);
			else if (number > PlacesList.Count)
				PlacesList.AddRange(Enumerable.Range(0, number - PlacesList.Count).Select(e => new PlayerPlace()));

			if(!groupSilent)
				NotifyLobbyChangedAsync();
		}

		public void ChangeScore(UserProfile caller, int placeIndex, int newScore)
		{
			if (caller != Master)
				throw new InvalidOperationException("There is no permission to change the score!");

			if (placeIndex >= PlacesList.Count)
				throw new InvalidOperationException("There's no such seat.");

			PlacesList[placeIndex].Score = newScore;

			NotifyLobbyChangedAsync();
		}

		public void SeatMaster(UserProfile user)
		{
			if (!HasUser(user))
				throw new InvalidOperationException("This user is not in the lobby");

			if (Master != null)
				throw new InvalidOperationException("The master's seat is already taken!");

			Unseat(user, true);

			Master = user;

			NotifyLobbyChangedAsync();
		}

		public void Seat(UserProfile user, int index)
		{
			if (!HasUser(user))
				throw new InvalidOperationException("This user is not in the lobby");

			if(index >= PlacesList.Count)
				throw new InvalidOperationException("There's no such seat.");

			if (PlacesList[index].User != null)
				throw new InvalidOperationException("The seat is already taken!");

			Unseat(user, true);

			PlacesList[index].User = user;

			NotifyLobbyChangedAsync();
		}

		private void Unseat(UserProfile user, bool groupSilent)
		{
			if (Master == user)
				Master = null;

			if (FindSeat(user, out PlayerPlace? table) && table != null)
				table.User = null;

			if (!groupSilent)
				NotifyLobbyChangedAsync();
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
			catch (Exception e)
			{
				Master = null;
				throw;
			}
		}

		public async Task JoinUserAsync(UserProfile user, CancellationToken ct)
		{
			if (user.CurrentLobby is {} currentLobby)
				throw new InvalidOperationException($"the user is already in lobby: {currentLobby.Name} ({currentLobby.Id})");

			UserList.Add(user);
			user.CurrentLobby = this;

			await _hubContext.Clients.Client(user.ConnectionId)
				.OnLobbyChanged(LobbyAction.Joined, LobbyData.Make(this));

			NotifyLobbyChangedAsync();

			await _hubContext.Groups.AddToGroupAsync(user.ConnectionId, Id.ToString(), ct);
			user.OnProfileChanged += NotifyLobbyChangedAsync;

			OnNumberUserChanged?.Invoke(this);
		}

		private async Task LeaveUserExtAsync(UserProfile user, bool groupSilent, CancellationToken ct)
		{
			Unseat(user, true);

			UserList.Remove(user);
			user.CurrentLobby = null;

			await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString(), ct);
			user.OnProfileChanged -= NotifyLobbyChangedAsync;

			await _hubContext.Clients.Client(user.ConnectionId)
				.OnLobbyChanged(LobbyAction.Leaved, null);

			if(!groupSilent)
				NotifyLobbyChangedAsync();

			OnNumberUserChanged?.Invoke(this);
		}

		public Task LeaveUserAsync(UserProfile user, CancellationToken ct)
		{
			return LeaveUserExtAsync(user, false, ct);
		}

		public async Task KickAllUsersAsync(CancellationToken ct = default)
		{
			if (UserList.Count == 0)
				return;

			var users = UserList.ToArray();

			var tasks = users.Select((u) => LeaveUserExtAsync(u, true, ct));

			await Task.WhenAll(tasks);

			OnNumberUserChanged?.Invoke(this);
		}

		private async void NotifyLobbyChangedAsync()
		{
			await _hubContext.Clients
				.Group(Id.ToString())
				.OnLobbyChanged(LobbyAction.Updated, LobbyData.Make(this))
				.ConfigureAwait(false);
		}
	}
}

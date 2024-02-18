using DobirnaGraServer.Hubs;
using DobirnaGraServer.Models.MessageTypes;
using DobirnaGraServer.Utils;
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

		public IEnumerable<ITable> Tables => TablesList;

		private List<UserProfile> UserList { get; init; } = [];

		private List<PlayerTable> TablesList { get; init; } = [];

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

		public bool FindSeat(UserProfile user, out PlayerTable? table)
		{
			var res = TablesList.FirstOrDefault(table => table.User == user);
			table = res;
			return res != null;
		}

		public void SetNumberSeats(int number)
		{
			TablesList.Capacity = number;
			if (number < TablesList.Count)
				TablesList.RemoveRange(number, TablesList.Count - number);
			else if (number > TablesList.Count)
				TablesList.AddRange(Enumerable.Repeat(new PlayerTable(), number - TablesList.Count));
		}

		public void SeatMaster(UserProfile user)
		{
			if (!HasUser(user))
				throw new InvalidOperationException("This user is not in the lobby");

			if (Master != null)
				throw new InvalidOperationException("The master's seat is already taken!");

			if (FindSeat(user, out PlayerTable? table) && table != null)
			{
				table.User = null;
			}

			Master = null;

			NotifyLobbyChangedAsync();
		}

		public void Seat(UserProfile user, int index)
		{
			if (!HasUser(user))
				throw new InvalidOperationException("This user is not in the lobby");

			if(index < TablesList.Count)
				throw new InvalidOperationException("There's no such seat.");

			if (TablesList[index].User != null)
				throw new InvalidOperationException("The seat is already taken!");

			if (FindSeat(user, out PlayerTable? table) && table != null)
			{
				table.User = null;
			}

			TablesList[index].User = user;

			NotifyLobbyChangedAsync();
		}

		public async Task JoinUserAsync(UserProfile user, CancellationToken ct)
		{
			if (user.CurrentLobby is {} currentLobby)
				throw new InvalidOperationException($"the user is already in lobby: {currentLobby.Name} ({currentLobby.Id})");

			UserList.Add(user);
			user.CurrentLobby = this;

			NotifyLobbyChangedAsync();

			await _hubContext.Clients.Client(user.ConnectionId)
				.OnLobbyChanged(LobbyAction.Joined, LobbyData.Make(this));

			await _hubContext.Groups.AddToGroupAsync(user.ConnectionId, Id.ToString(), ct);
			user.OnProfileChanged += NotifyLobbyChangedAsync;

			OnNumberUserChanged?.Invoke(this);
		}

		private async Task LeaveUserExtAsync(UserProfile user, bool groupSilent, CancellationToken ct)
		{
			UserList.Remove(user);
			user.CurrentLobby = null;

			await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString(), ct);
			user.OnProfileChanged -= NotifyLobbyChangedAsync;

			await _hubContext.Clients.Client(user.ConnectionId)
				.OnLobbyChanged(LobbyAction.Leaved, null);

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
				.OnLobbyChanged(LobbyAction.Updated, LobbyData.Make(this));
		}
	}
}

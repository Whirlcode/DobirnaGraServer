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

		public InviteCode InviteCode { get; init; }

		public IEnumerable<IProfile> Users => UserList;

		private WeakList<UserProfile> UserList { get; init; }

		public int NumberUser => UserList.Count;

		public string Name { get; init; }

		public Lobby(IHubContext<GameHub, IGameClient> hubContext, string name)
		{
			_hubContext = hubContext;
			Name = name;

			Id = Guid.NewGuid();
			InviteCode = new InviteCode(Id);
			UserList = new WeakList<UserProfile>();
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

		public async Task JoinUserAsync(UserProfile user, CancellationToken ct)
		{
			if (user.CurrentLobby is {} currentLobby)
				throw new InvalidOperationException($"the user is already in lobby: {currentLobby.Name} ({currentLobby.Id})");

			UserList.Add(user);
			user.CurrentLobby = this;

			await _hubContext.Groups.AddToGroupAsync(user.ConnectionId, Id.ToString(), ct);

			OnNumberUserChanged?.Invoke(this);

			NotifyGameStateChangedAsync();
		}

		public async Task LeaveUserAsync(UserProfile user, CancellationToken ct)
		{
			UserList.Remove(user);
			user.CurrentLobby = null;

			await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString(), ct);

			OnNumberUserChanged?.Invoke(this);

			NotifyGameStateChangedAsync();
		}

		public async Task KickAllUsersAsync(CancellationToken ct = default)
		{
			var users = UserList.ToArray();
			UserList.Clear();
			foreach (var user in users)
			{
				await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString(), ct);
				user.CurrentLobby = null;
			}

			OnNumberUserChanged?.Invoke(this);

			NotifyGameStateChangedAsync();
		}

		private async void NotifyGameStateChangedAsync()
		{
			await _hubContext.Clients
				.Group(Id.ToString())
				.OnLobbyStateChanged(LobbyInfo.Make(this));
		}
	}
}

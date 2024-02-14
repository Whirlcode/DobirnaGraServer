using DobirnaGraServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public sealed class LobbyInstance : IDisposable, IAsyncDisposable
	{
		public delegate void EventNumberUserChanged(LobbyInstance lobby);

		public event EventNumberUserChanged? OnNumberUserChanged;

		public Guid Id { get; init; }

		private readonly IHubContext<GameHub> _hubContext;

		private InviteCode InviteCode { get; init; }

		private UserList Users { get; init; }

		public int NumberUser => Users.Count;

		public string Name { get; init; }

		public LobbyInstance(IHubContext<GameHub> hubContext, string name)
		{
			_hubContext = hubContext;
			Name = name;

			Id = Guid.NewGuid();
			InviteCode = new InviteCode(Id);
			Users = new UserList();
		}

		~LobbyInstance()
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

		public bool HasUser(UserInstance user)
		{
			return Users.Any(e => e == user);
		}

		public async Task JoinUserAsync(UserInstance user)
		{
			if (user.CurrentLobby is {} currentLobby)
				throw new InvalidOperationException($"the user is already in lobby: {currentLobby.Name} ({currentLobby.Id})");

			Users.AddUser(user);
			user.CurrentLobby = this;

			await _hubContext.Groups.AddToGroupAsync(user.ConnectionId, Id.ToString());

			OnNumberUserChanged?.Invoke(this);
		}

		public async Task LeaveUserAsync(UserInstance user)
		{
			Users.RemoveUser(user);
			user.CurrentLobby = null;

			await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString());

			OnNumberUserChanged?.Invoke(this);
		}

		public async Task KickAllUsersAsync()
		{
			var users = Users.ToArray();
			Users.Clear();
			foreach (var user in users)
			{
				await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString());
				user.CurrentLobby = null;
			}

			OnNumberUserChanged?.Invoke(this);
		}
	}
}

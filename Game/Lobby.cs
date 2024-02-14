using DobirnaGraServer.Hubs;
using DobirnaGraServer.Utils;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
    public sealed class Lobby : IDisposable, IAsyncDisposable
	{
		public delegate void EventNumberUserChanged(Lobby lobby);

		public event EventNumberUserChanged? OnNumberUserChanged;

		public Guid Id { get; init; }

		private readonly IHubContext<GameHub> _hubContext;

		private InviteCode InviteCode { get; init; }

		private WeakList<UserProfile> Users { get; init; }

		public int NumberUser => Users.Count;

		public string Name { get; init; }

		public Lobby(IHubContext<GameHub> hubContext, string name)
		{
			_hubContext = hubContext;
			Name = name;

			Id = Guid.NewGuid();
			InviteCode = new InviteCode(Id);
			Users = new WeakList<UserProfile>();
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
			return Users.Any(e => e == user);
		}

		public async Task JoinUserAsync(UserProfile user, CancellationToken ct)
		{
			if (user.CurrentLobby is {} currentLobby)
				throw new InvalidOperationException($"the user is already in lobby: {currentLobby.Name} ({currentLobby.Id})");

			Users.Add(user);
			user.CurrentLobby = this;

			await _hubContext.Groups.AddToGroupAsync(user.ConnectionId, Id.ToString(), ct);

			OnNumberUserChanged?.Invoke(this);
		}

		public async Task LeaveUserAsync(UserProfile user, CancellationToken ct)
		{
			Users.Remove(user);
			user.CurrentLobby = null;

			await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString(), ct);

			OnNumberUserChanged?.Invoke(this);
		}

		public async Task KickAllUsersAsync(CancellationToken ct = default)
		{
			var users = Users.ToArray();
			Users.Clear();
			foreach (var user in users)
			{
				await _hubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, Id.ToString(), ct);
				user.CurrentLobby = null;
			}

			OnNumberUserChanged?.Invoke(this);
		}

		private void NotifyGameStateChanged()
		{
			
		}
	}
}

using DobirnaGraServer.Hubs;
using DobirnaGraServer.Models.MessageTypes;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public class UserProfile : IProfile
	{
		private readonly string _connectionId;

		public string ConnectionId => _connectionId;

		private readonly WeakReference _weakContext;

		private HubCallerContext? UserContext => _weakContext.IsAlive ? _weakContext.Target as HubCallerContext : null;

		private readonly IHubContext<GameHub, IGameClient> _hubContext;

		public Guid Id { get; private init; } = new Guid();

		public string Name { get; set; } = string.Empty;

		private Lobby? _currentLobby;
		public Lobby? CurrentLobby
		{
			get => _currentLobby;
			set
			{
				if (UserContext == null)
					throw new InvalidOperationException("User is dead!");
				if (value != null && !value.HasUser(this))
					throw new InvalidOperationException("The user is not in the group.");
				if (value == null && _currentLobby != null && _currentLobby.HasUser(this))
					throw new InvalidOperationException("The user is still in the group.");

				_currentLobby = value;

				NotifyOnProfileChanged();
			}
		}

		public UserProfile(HubCallerContext callerContext, IHubContext<GameHub, IGameClient> hubContext)
		{
			_connectionId = callerContext.ConnectionId;
			_weakContext = new WeakReference(callerContext);
			_hubContext = hubContext;
		}

		public async Task AbandonAsync()
		{
			if (CurrentLobby != null)
			{
				await CurrentLobby.LeaveUserAsync(this, default);
			}
			CurrentLobby = null;
			_weakContext.Target = null;
		}

		public async void NotifyOnProfileChanged()
		{
			await _hubContext.Clients.Clients(_connectionId).OnProfileChanged(UserInfo.Make(this));
		}
	}
}

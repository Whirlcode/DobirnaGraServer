using DobirnaGraServer.Game;
using DobirnaGraServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class ProfileService(IHubContext<GameHub, IGameClient> hubContext)
	{
		private readonly Dictionary<string, UserProfile> _users = new();

		private readonly object _lockUsers = new();

		public async Task<UserProfile> RegisterAsync(HubCallerContext caller)
		{
			UserProfile profile = new(hubContext);

			lock (_lockUsers)
			{
				_users.Add(caller.ConnectionId, profile);
			}

			await profile.Login(caller);

			return profile;
		}

		public async Task UnregisterAsync(HubCallerContext caller)
		{
			UserProfile? instance;

			lock (_lockUsers)
			{
				_users.Remove(caller.ConnectionId, out instance);
			}

			if (instance != null)
			{
				await instance.Logout();
			}
		}
	}
}

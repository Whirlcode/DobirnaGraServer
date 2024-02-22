using DobirnaGraServer.Game;
using DobirnaGraServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class ProfileService(IHubContext<GameHub, IGameClient> hubContext)
	{
		private readonly Dictionary<Guid, UserProfile> _users = new();

		private readonly object _lockUsers = new();

		public async Task<UserProfile> RegisterAsync(HubCallerContext caller)
		{
			UserProfile profile = new(hubContext);

			lock (_lockUsers)
			{
				_users.Add(profile.Id, profile);
			}

			await profile.Login(caller);

			return profile;
		}

		public async Task UnregisterAsync(Guid id)
		{
			UserProfile? instance;

			lock (_lockUsers)
			{
				_users.Remove(id, out instance);
			}

			if (instance != null)
			{
				await instance.Logout();
			}
		}

		public bool FindUser(Guid id, out UserProfile? user)
		{
			return _users.TryGetValue(id, out user);
		}

		public int NumberUsers => _users.Count;
	}
}

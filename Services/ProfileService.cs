using DobirnaGraServer.Game;
using DobirnaGraServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class ProfileService(IHubContext<GameHub, IGameClient> hubContext)
	{
		private readonly Dictionary<string, UserProfile> _persistentData = new();

		public Task<UserProfile> RegisterAsync(HubCallerContext caller)
		{
			UserProfile profile = new(caller, hubContext);
			_persistentData.Add(caller.ConnectionId, profile);
			return Task.FromResult(profile);
		}

		public async Task UnregisterAsync(HubCallerContext caller)
		{
			_persistentData.Remove(caller.ConnectionId, out UserProfile? instance);
			if (instance != null)
			{
				await instance.AbandonAsync();
			}
		}
	}
}

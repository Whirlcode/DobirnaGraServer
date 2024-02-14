using DobirnaGraServer.Game;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class UserService
	{
		private Dictionary<string, UserInstance> persistentData = new();

		public Task<UserInstance> RegisterAsync(HubCallerContext caller)
		{
			UserInstance instance = new(caller);
			persistentData.Add(caller.ConnectionId, instance);
			return Task.FromResult(instance);
		}

		public async Task UnregisterAsync(HubCallerContext caller)
		{
			persistentData.Remove(caller.ConnectionId, out UserInstance? instance);
			if (instance != null)
			{
				await instance.AbandonAsync();
			}
		}
	}
}

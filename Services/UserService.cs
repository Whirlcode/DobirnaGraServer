using DobirnaGraServer.Game;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class UserService
	{
		private Dictionary<string, UserInstance> persistentData = new();

		public UserInstance Register(HubCallerContext caller)
		{
			UserInstance instance = new(caller);
			persistentData.Add(caller.ConnectionId, instance);
			return instance;
		}

		public void Unregister(HubCallerContext caller)
		{
			persistentData.Remove(caller.ConnectionId, out UserInstance? instance);
			instance?.Abandon();
		}
	}
}

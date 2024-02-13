using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public class UserInstance(HubCallerContext context)
	{
		private WeakReference WeakContext { get; set; } = new(context);

		private HubCallerContext? Context => WeakContext.IsAlive ? WeakContext.Target as HubCallerContext : null;

		public bool IsValid => Context != null;

		public void Abandon()
		{
			WeakContext.Target = null;
		}
	}
}

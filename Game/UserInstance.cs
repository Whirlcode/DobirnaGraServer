using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public class UserInstance(HubCallerContext context)
	{
		private WeakReference WeakContext { get; set; } = new(context);

		private HubCallerContext? Context => WeakContext.IsAlive ? WeakContext.Target as HubCallerContext : null;

		public bool IsValid => Context != null;

		public LobbyInstance? CurrentLobby
		{
			get => (LobbyInstance?)Context?.Items[nameof(CurrentLobby)];
			set
			{
				if (Context == null)
					throw new InvalidOperationException("User is dead!");
				if (value != null && !value.HasUser(this))
					throw new InvalidOperationException("The user is not in the group.");
				Context.Items[nameof(CurrentLobby)] = value;
			}
		}

		public void Abandon()
		{
			WeakContext.Target = null;
		}
	}
}

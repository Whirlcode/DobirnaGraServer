using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public class UserProfile(HubCallerContext context)
	{
		public string ConnectionId => context.ConnectionId;

		private WeakReference WeakContext { get; set; } = new(context);

		private HubCallerContext? Context => WeakContext.IsAlive ? WeakContext.Target as HubCallerContext : null;

		public bool IsValid => Context != null;

		public Lobby? CurrentLobby
		{
			get => (Lobby?)Context?.Items[nameof(CurrentLobby)];
			set
			{
				if (Context == null)
					throw new InvalidOperationException("User is dead!");
				if (value != null && !value.HasUser(this))
					throw new InvalidOperationException("The user is not in the group.");
				if (value == null && Context.Items[nameof(CurrentLobby)] is Lobby current && current.HasUser(this))
					throw new InvalidOperationException("The user is still in the group.");

				Context.Items[nameof(CurrentLobby)] = value;
			}
		}

		public async Task AbandonAsync()
		{
			if (CurrentLobby != null)
			{
				await CurrentLobby.LeaveUserAsync(this, default);
			}
			CurrentLobby = null;
			WeakContext.Target = null;
		}
	}
}

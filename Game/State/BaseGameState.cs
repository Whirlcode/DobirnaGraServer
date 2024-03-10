using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game.State
{
	public delegate void StateEvent();

	public interface IState
	{
		public Lobby OwnerLobby { get; init; }

		void OnEnter();

		void OnExit();

		BaseStateData GetStateData();
	}

	public abstract class BaseGameState : IState
	{
		public required Lobby OwnerLobby { get; init; }

		public virtual void OnEnter()
		{
			OwnerLobby.NotifyGameStateChangedAsync(GetStateData(), true);
		}

		public virtual void OnExit()
		{
			
		}

		public abstract BaseStateData GetStateData();

		protected void NotifyStateChanged()
		{
			OwnerLobby.NotifyGameStateChangedAsync(GetStateData(), false);
		}
	}
}

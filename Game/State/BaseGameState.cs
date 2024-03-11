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
			OwnerLobby.RpcClients()
				.OnGameStateChanged(GameStateAction.Entered, GetStateData())
				.ConfigureAwait(false);
		}

		public virtual void OnExit()
		{
			
		}

		public abstract BaseStateData GetStateData();

		protected void NotifyStateChanged()
		{
			OwnerLobby.RpcClients()
				.OnGameStateChanged(GameStateAction.Updated, GetStateData())
				.ConfigureAwait(false);
		}
	}
}

using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game.State
{
	public class IdleGameState : BaseGameState
	{
		public event StateEvent? OnStartGame;

		private HashSet<IProfile> ReadyUsers { get; init; } = new();

		public override void OnEnter()
		{
			base.OnEnter();

			ReadyUsers.Clear();
		}

		public override void OnExit()
		{
			base.OnExit();

			ReadyUsers.Clear();
		}

		public override BaseStateData GetStateData()
		{
			return new IdleStateData()
			{
				ReadyUsers = ReadyUsers.Select(r => r.Id).ToList()
			};
		}

		public void ToggleReady(IProfile caller)
		{
			if (ReadyUsers.Contains(caller))
			{
				ReadyUsers.Remove(caller);
			}
			else
			{
				ReadyUsers.Add(caller);
			}

			NotifyStateChanged();
		}

		public void StartGame()
		{
			OnStartGame?.Invoke();
		}
	}
}

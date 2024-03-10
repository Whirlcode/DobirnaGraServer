using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game.State
{
	public class IdleGameState : BaseGameState
	{
		public event StateEvent? OnStartGame;

		private HashSet<IProfile> ReadyUsers { get; init; } = new();

		public override void OnEnter()
		{
			ReadyUsers.Clear();

			base.OnEnter();

			OwnerLobby.OnUserChanged += OnUserChanged;
		}

		public override void OnExit()
		{
			base.OnExit();

			ReadyUsers.Clear();

			OwnerLobby.OnUserChanged -= OnUserChanged;
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

		private void OnUserChanged(Lobby lobby, IProfile profile, Lobby.UserAction action)
		{
			if (action == Lobby.UserAction.Leaved)
			{
				ReadyUsers.Remove(profile);
			}
		}
	}
}

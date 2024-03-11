using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game.State
{
	public class RoundGameState : BaseGameState
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override BaseStateData GetStateData()
		{
			return new RoundStateData
			{
				Questions = new()
				{
					{ "Game", [100, 200, 300, 400, 500] },
					{ "Music", [100, 200, 300, 400, 500] },
					{ "C++?", [100, 200, 300, 400, 500] },
					{ "JavaChad", [100, 200, 300, 400, 500, 600, 700] },
				},
				Electioneer = OwnerLobby.Users.Last().Id
			};
		}
	}
}

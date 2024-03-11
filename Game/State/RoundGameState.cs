using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game.State
{
	public class RoundGameState : BaseGameState
	{
		private Guid? _currentElectioneer;

		public override void OnEnter()
		{
			_currentElectioneer = OwnerLobby.Places
				.OrderBy(p => p.Score)
				.FirstOrDefault(p => p.User != null)?.User?.Id;

			base.OnEnter();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override BaseStateDataRpcMessage GetStateData()
		{
			return new RoundStateDataRpcMessageRpcMessage
			{
				Questions = new()
				{
					{ "Game", [100, 200, 300, 400, 500] },
					{ "Music", [100, 200, 300, 400, 500] },
					{ "C++?", [100, 200, 300, 400, 500] },
					{ "JavaChad", [100, 200, 300, 400, 500, 600, 700] },
				},
				Electioneer = _currentElectioneer
			};
		}
	}
}

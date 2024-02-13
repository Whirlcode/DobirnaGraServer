namespace DobirnaGraServer.Models.RequestTypes
{
	public class JoinLobbyRequest
	{
		public required string InviteCode { get; init; }
	}

	public class CreateLobbyRequest
	{
		public required string Name { get; init; }
	}

	public class GameState
	{

	}
}

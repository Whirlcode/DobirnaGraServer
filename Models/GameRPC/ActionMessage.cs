namespace DobirnaGraServer.Models.GameRPC
{
	public class JoinLobbyActionMessage
	{
		public required string InviteCode { get; init; }

		public required string UserName { get; init; }
	}

	public class CreateLobbyActionMessage
	{
		public required string NameLobby { get; init; }

		public required string UserName { get; init; }

		public required int InitialNumberPlaces { get; init; }
	}

	public class UpdateProfileActionMessage
	{
		public string? Name { get; init; }
	}

	public class ChangeScoreActionMessage
	{
		public required int TargetPlaceIndex { get; init; }

		public required int NewScore { get; init; }
	}
}

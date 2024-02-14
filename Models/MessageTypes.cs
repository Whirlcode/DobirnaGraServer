namespace DobirnaGraServer.Models.MessageTypes
{
	public class JoinLobbyActionMessage
	{
		public required string InviteCode { get; init; }
	}

	public class CreateLobbyActionMessage
	{
		public required string Name { get; init; }
	}

	public class UserInfo
	{
		public required Guid Id { get; init; }

		public required string Name { get; init; }
	}

	public class LobbyInfo
	{
		public required Guid LobbyId { get; init; }

		public required string Name { get; init; }

		public string? InviteCode { get; init; }

		public IList<UserInfo>? Users { get; init; }
	}
}

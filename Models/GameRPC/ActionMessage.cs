namespace DobirnaGraServer.Models.GameRPC
{
	public readonly struct JoinLobbyActionMessage
	{
		public required string InviteCode { get; init; }

		public required string UserName { get; init; }
	}

	public readonly struct CreateLobbyActionMessage
	{
		public required string NameLobby { get; init; }

		public required string UserName { get; init; }

		public required int InitialNumberPlaces { get; init; }
	}

	public readonly struct UpdateProfileActionMessage
	{
		public string? Name { get; init; }
	}

	public readonly struct ChangeScoreActionMessage
	{
		public required int TargetPlaceIndex { get; init; }

		public required int NewScore { get; init; }
	}
}

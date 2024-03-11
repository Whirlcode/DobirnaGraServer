namespace DobirnaGraServer.Models.GameRPC
{
	public readonly struct PlayerPlaceDataRpcMessage
	{
		public Guid? UserId { get; init; }

		public string? UserName { get; init; }

		public required int Score { get; init; }

		public string? ImageId { get; init; }

		public required bool IsOccupied { get; init; }
	}

	public readonly struct MasterDataRpcMessage
	{
		public Guid? UserId { get; init; }

		public string? UserName { get; init; }

		public string? ImageId { get; init; }

		public required bool IsOccupied { get; init; }
	}

	public enum LobbyAction
	{
		Joined,
		Leaved,
		Updated
	}

	public readonly struct LobbyDataRpcMessage
	{
		public required Guid Id { get; init; }

		public required string Name { get; init; }

		public required string InviteCode { get; init; }

		public required IList<PlayerPlaceDataRpcMessage> Places { get; init; }

		public required MasterDataRpcMessage Master { get; init; }
	}
}

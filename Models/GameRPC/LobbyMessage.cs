using DobirnaGraServer.Game;

namespace DobirnaGraServer.Models.GameRPC
{
	public readonly struct PlayerPlaceData
	{
		public Guid? UserId { get; init; }

		public string? UserName { get; init; }

		public required int Score { get; init; }

		public string? ImageId { get; init; }

		public required bool IsOccupied { get; init; }
	}

	public readonly struct MasterData
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

	public readonly struct LobbyData
	{
		public required Guid Id { get; init; }

		public required string Name { get; init; }

		public required string InviteCode { get; init; }

		public required IList<PlayerPlaceData> Places { get; init; }

		public required MasterData Master { get; init; }
	}
}

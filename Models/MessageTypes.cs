using DobirnaGraServer.Game;

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

	public class UpdateProfileActionMessage
	{
		public string? Name { get; init; }
	}

	public enum ProfileAction
	{
		LoggedIn,
		Updated,
		Logout
	}

	public class ProfileData
	{
		public required Guid Id { get; init; }

		public static ProfileData Make(IProfile me)
		{
			return new ProfileData()
			{
				Id = me.Id,
			};
		}
	}

	public class PlayerTableData
	{
		public Guid Id { get; init; }

		public string Name { get; init; } = string.Empty;

		public int Score { get; init; }

		public static PlayerTableData Make(ITable table)
		{
			return new PlayerTableData()
			{
				Id = table.User?.Id ?? Guid.Empty,
				Name = table.User?.Name ?? string.Empty,
				Score = table.Score
			};
		}
	}

	public class MasterData
	{
		public required Guid Id { get; init; }

		public string Name { get; init; } = string.Empty;

		public static MasterData Make(IProfile? profile)
		{
			return new MasterData()
			{
				Id = profile?.Id ?? Guid.Empty,
				Name = profile?.Name ?? String.Empty
			};
		}
	}

	public enum LobbyAction
	{
		Joined,
		Leaved,
		Updated
	}

	public class LobbyData
	{
		public required Guid Id { get; init; }

		public required string Name { get; init; }

		public required string InviteCode { get; init; }

		public required IList<PlayerTableData> Tables { get; init; }

		public required MasterData Master { get; init; }

		public static LobbyData Make(ILobby lobby)
		{
			return new LobbyData()
			{
				Id = lobby.Id,
				Name = lobby.Name,
				InviteCode = lobby.InviteCode,
				Tables = lobby.Tables.Select(PlayerTableData.Make).ToList(),
				Master = MasterData.Make(lobby.Master)
			};
		}
	}
}

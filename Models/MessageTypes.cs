using DobirnaGraServer.Game;

namespace DobirnaGraServer.Models.MessageTypes
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

	public class PlayerPlaceData
	{
		public Guid? UserId { get; init; }

		public string? UserName { get; init; }

		public required int Score { get; init; }

		public required bool IsOccupied { get; init; }

		public static PlayerPlaceData Make(ITable table)
		{
			return new PlayerPlaceData()
			{
				UserId = table.User?.Id,
				UserName = table.User?.Name,
				Score = table.Score,
				IsOccupied = table.User != null
			};
		}
	}

	public class MasterData
	{
		public Guid? UserId { get; init; }

		public string? UserName { get; init; } = string.Empty;

		public required bool IsOccupied { get; init; }

		public static MasterData Make(IProfile? profile)
		{
			return new MasterData()
			{
				UserId = profile?.Id,
				UserName = profile?.Name,
				IsOccupied = profile != null
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

		public required IList<PlayerPlaceData> Places { get; init; }

		public required MasterData Master { get; init; }

		public static LobbyData Make(ILobby lobby)
		{
			return new LobbyData()
			{
				Id = lobby.Id,
				Name = lobby.Name,
				InviteCode = lobby.InviteCode,
				Places = lobby.Places.Select(PlayerPlaceData.Make).ToList(),
				Master = MasterData.Make(lobby.Master)
			};
		}
	}
}

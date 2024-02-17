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

	public class UserInfo
	{
		public required Guid Id { get; init; }

		public required string Name { get; init; }

		public required GameRole Role { get; init; }

		public static UserInfo Make(IProfile me)
		{
			return new UserInfo()
			{
				Id = me.Id,
				Name = me.Name,
				Role = me.Role
			};
		}
	}

	public enum LobbyAction
	{
		Joined,
		Leaved,
		Updated
	}

	public class LobbyInfo
	{
		public required Guid Id { get; init; }

		public required string Name { get; init; }

		public required string InviteCode { get; init; }

		public required IList<UserInfo> Users { get; init; }

		public static LobbyInfo Make(ILobby lobby)
		{
			return new LobbyInfo()
			{
				Id = lobby.Id,
				Name = lobby.Name,
				InviteCode = lobby.InviteCode,
				Users = lobby.Users.Select(UserInfo.Make).ToList()
			};
		}
	}
}

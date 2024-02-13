namespace DobirnaGraServer.Game
{
	public sealed class LobbyInstance : IDisposable
	{
		public Guid Id { get; init; }

		public InviteCode InviteCode { get; init; }

		public UserList Users { get; init; }

		public LobbyInstance()
		{
			Id = Guid.NewGuid();
			InviteCode = new InviteCode(Id);
			Users = new UserList();
		}

		~LobbyInstance()
		{
			Dispose();
		}

		public void Dispose()
		{
			Users.Clear();
			InviteCode.Dispose();
		}

		public bool HasUser(UserInstance user)
		{
			return Users.Any(e => e == user);
		}

		public void JoinUser(UserInstance user)
		{
			Users.AddUser(user);
			user.CurrentLobby = this;
		}

		public void LeaveUser(UserInstance user)
		{
			Users.RemoveUser(user);
			user.CurrentLobby = null;
		}
	}
}

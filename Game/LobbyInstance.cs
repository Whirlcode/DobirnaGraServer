namespace DobirnaGraServer.Game
{
	public sealed class LobbyInstance : IDisposable
	{
		public delegate void EventNumberUserChanged(LobbyInstance lobby);

		public event EventNumberUserChanged? OnNumberUserChanged;

		public Guid Id { get; init; }

		public InviteCode InviteCode { get; init; }

		public UserList Users { get; init; }

		public int NumberUser => Users.Count;

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
			KickAllUsers();
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

			OnNumberUserChanged?.Invoke(this);
		}

		public void LeaveUser(UserInstance user)
		{
			Users.RemoveUser(user);
			user.CurrentLobby = null;

			OnNumberUserChanged?.Invoke(this);
		}

		private void KickAllUsers()
		{
			var users = Users.ToArray();
			Users.Clear();
			foreach (var user in users)
			{
				user.CurrentLobby = null;
			}
		}
	}
}

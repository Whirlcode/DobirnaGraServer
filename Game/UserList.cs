using System.Collections;

namespace DobirnaGraServer.Game
{
	public class UserList : IEnumerable<UserProfile>
	{
		private List<WeakReference> _users = new();

		public int Count => _users.Count;

		public void AddUser(UserProfile user)
		{
			_users.Add(new WeakReference(user));
			Validate();
		}

		public void RemoveUser(UserProfile user)
		{
			_users.RemoveAll(u => u.Target == user);
			Validate();
		}

		public void Clear()
		{
			_users.Clear();
		}

		private void Validate()
		{
			_users.RemoveAll(u => u.IsAlive == false);
		}

		public IEnumerator<UserProfile> GetEnumerator()
		{
			foreach(var weak in _users)
			{
				if (weak is { IsAlive: true, Target: UserProfile user })
				{
					yield return user;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

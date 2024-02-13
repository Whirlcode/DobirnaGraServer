using System.Collections;

namespace DobirnaGraServer.Game
{
	public class UserList : IEnumerable<UserInstance>
	{
		private List<WeakReference> _users = new();

		public int Count => _users.Count;

		public void AddUser(UserInstance user)
		{
			_users.Add(new WeakReference(user));
			Validate();
		}

		public void RemoveUser(UserInstance user)
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

		public IEnumerator<UserInstance> GetEnumerator()
		{
			foreach(var weak in _users)
			{
				if (weak is { IsAlive: true, Target: UserInstance user })
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

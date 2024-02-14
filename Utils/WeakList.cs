using System.Collections;

namespace DobirnaGraServer.Utils
{
    public class WeakList<T> : IEnumerable<T>
    {
        private List<WeakReference> _users = new();

        public int Count => _users.Count;

        public void Add(T item)
        {
            _users.Add(new WeakReference(item));
            Validate();
        }

        public void Remove(object user)
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

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var weak in _users)
            {
                if (weak is { IsAlive: true, Target: T item })
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

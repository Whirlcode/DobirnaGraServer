using DobirnaGraServer.Assets;

namespace DobirnaGraServer.Game
{
	public interface IProfile
	{
		Guid Id { get; }

		string Name { get; }

		public ScopeFile? Avatar { get; }
	}
}

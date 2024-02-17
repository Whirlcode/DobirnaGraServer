namespace DobirnaGraServer.Game
{
	public interface IProfile
	{
		Guid Id { get; }

		string Name { get; }

		GameRole Role { get; }
	}
}

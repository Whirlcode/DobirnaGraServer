namespace DobirnaGraServer.Game
{
	public interface ILobby
	{
		Guid Id { get; }

		string Name { get; }

		InviteCode InviteCode { get; }

		IEnumerable<IProfile> Users { get; }

		IEnumerable<IPlace> Places { get; }

		IProfile? Master { get; }
	}
}

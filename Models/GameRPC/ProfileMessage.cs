using DobirnaGraServer.Game;

namespace DobirnaGraServer.Models.GameRPC
{
	public enum ProfileAction
	{
		LoggedIn,
		Updated,
		Logout
	}

	public readonly struct ProfileData
	{
		public required Guid Id { get; init; }
	}
}

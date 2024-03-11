using DobirnaGraServer.Game;

namespace DobirnaGraServer.Models.GameRPC
{
	public enum ProfileAction
	{
		LoggedIn,
		Updated,
		Logout
	}

	public readonly struct ProfileDataRpcMessage
	{
		public required Guid Id { get; init; }
	}
}

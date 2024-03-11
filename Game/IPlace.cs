using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game
{
	public interface IPlace
	{
		public int Score { get; }

		public IProfile? User { get; }

		public PlayerPlaceData ConvertToRpcData();
	}
}

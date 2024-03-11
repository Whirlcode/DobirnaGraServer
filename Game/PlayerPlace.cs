using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game
{
	public class PlayerPlace 
	{
		public int Score { get; set; }

		public UserProfile? User { get; set; }

		public PlayerPlaceDataRpcMessage ConvertToRpcData()
		{
			return new PlayerPlaceDataRpcMessage
			{
				UserId = User?.Id,
				UserName = User?.Name,
				Score = Score,
				ImageId = $"{User?.Avatar?.Id}",
				IsOccupied = User != null
			};
		}
	}
}

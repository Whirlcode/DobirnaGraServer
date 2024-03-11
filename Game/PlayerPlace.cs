using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Game
{
	public class PlayerPlace 
	{
		public int Score { get; set; }

		public UserProfile? User { get; set; }

		public PlayerPlaceData ConvertToRpcData()
		{
			return new PlayerPlaceData
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

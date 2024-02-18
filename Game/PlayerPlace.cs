namespace DobirnaGraServer.Game
{
	public class PlayerPlace : IPlace
	{
		public int Score { get; set; }

		public IProfile? User { get; set; }
	}
}

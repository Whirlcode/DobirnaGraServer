namespace DobirnaGraServer.Game
{
	public class PlayerPlace : ITable
	{
		public int Score { get; set; }

		public IProfile? User { get; set; }
	}
}

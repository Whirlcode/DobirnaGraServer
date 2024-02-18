namespace DobirnaGraServer.Game
{
	public class PlayerTable : ITable
	{
		public int Score { get; set; }

		public IProfile? User { get; set; }
	}
}

namespace DobirnaGraServer.Game
{
	public interface ITable
	{
		public int Score { get; }

		public IProfile? User { get; }
	}
}

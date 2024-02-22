namespace DobirnaGraServer.Utils
{
	public static class IntExtension
	{
		private enum Kinds
		{
			Bytes = 0,
			Kilobytes = 1,
			Megabytes = 2,
			Gigabytes = 3,
			Terabytes = 4,
		}

		public static int Megabytes(this int value)
		{
			return (int)(value * Math.Pow(1024, (int)Kinds.Megabytes));
		}
	}
}

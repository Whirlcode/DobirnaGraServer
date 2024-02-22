namespace DobirnaGraServer.Utils
{
	public static class ContentTypeExtensions
	{
		public static readonly string[] ImageTypes = new[]
		{
			"image/jpg",
			"image/jpeg",
			"image/pjpeg",
			"image/gif",
			"image/x-png",
			"image/png"
		};

		public static bool IsImageType(string str)
		{
			return ImageTypes.Any(s => s == str);
		}
	}
}

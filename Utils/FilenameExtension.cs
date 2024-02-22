
namespace DobirnaGraServer.Utils
{
	public static class FilenameExtension 
	{
		public static readonly string[] ImageExtensions = new[]
		{
			".jpg",
			".png",
			".gif",
			".jpeg",
		};

		public static bool IsImage(string path)
		{
			var ext = Path.GetExtension(path);
			return ImageExtensions.Any(s => s == ext);
		}
	}
}

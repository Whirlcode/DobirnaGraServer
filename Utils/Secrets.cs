using System.Security.Cryptography;

namespace DobirnaGraServer.Utils
{
	public static class Secrets
	{
		public static string GenerateTokenUrlSafe(int length)
		{
			using var rng = RandomNumberGenerator.Create();
			byte[] bytes = new byte[length];
			rng.GetBytes(bytes);
			return Convert.ToBase64String(bytes)
				.TrimEnd('=')
				.Replace('+', '-')
				.Replace('/', '_');
		}
	}
}

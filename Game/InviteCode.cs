using DobirnaGraServer.Utils;

namespace DobirnaGraServer.Game
{
	public sealed class InviteCode : IDisposable
	{
		public string Code { get; init; }

		public InviteCode(Guid id)
		{
			Code = GenerateUniqueToken();

			AssociatedCodes.Add(Code, id);
		}

		~InviteCode()
		{
			Dispose();
		}

		public void Dispose()
		{
			AssociatedCodes.Remove(Code);
		}

		private static readonly IDictionary<string, Guid> AssociatedCodes = new Dictionary<string, Guid>();

		private static string GenerateUniqueToken()
		{
			string token;

			do
			{
				token = Secrets.GenerateTokenUrlSafe(6);
			} while (AssociatedCodes.ContainsKey(token));

			return token;
		}

		public static bool FindId(string inviteCode, out Guid id) => AssociatedCodes.TryGetValue(inviteCode, out id);

		public static implicit operator string(InviteCode invite) => invite.Code;
	}
}

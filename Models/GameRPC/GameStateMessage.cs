using System.Text.Json.Serialization;

namespace DobirnaGraServer.Models.GameRPC
{
	public enum GameStateAction
	{
		Entered,
		Updated,
	}


	[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
	[JsonDerivedType(typeof(BaseStateData), typeDiscriminator: "base")]
	[JsonDerivedType(typeof(IdleStateData), typeDiscriminator: "idle")]
	public class BaseStateData
	{
		public string Type { get; init; } // hack for .NET 8, because SignalR not support JsonPolymorphic

		protected BaseStateData(string type)
		{
			Type = type;
		}
	}

	public class IdleStateData : BaseStateData
	{
		public IdleStateData()
			: base("idle")
		{}

		public required List<Guid> ReadyUsers { get; init; }
	}
}

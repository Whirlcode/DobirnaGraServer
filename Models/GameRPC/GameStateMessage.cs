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
	[JsonDerivedType(typeof(IdleStateData), typeDiscriminator: "round")]
	public class BaseStateData(string type)
	{
		public string Type { get; init; } = type; // hack for .NET 8, because SignalR not support JsonPolymorphic
	}

	public class IdleStateData() : BaseStateData("idle")
	{
		public required List<Guid> ReadyUsers { get; init; }
	}


	public class RoundStateData() : BaseStateData("round")
	{
		public required Dictionary<string, List<int>> Questions;

		public required Guid? Electioneer;
	}
}

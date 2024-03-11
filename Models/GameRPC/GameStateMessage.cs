using System.Text.Json.Serialization;

namespace DobirnaGraServer.Models.GameRPC
{
	public enum GameStateAction
	{
		Entered,
		Updated,
	}


	[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
	[JsonDerivedType(typeof(BaseStateDataRpcMessage), typeDiscriminator: "base")]
	[JsonDerivedType(typeof(IdleStateDataRpcMessageRpcMessage), typeDiscriminator: "idle")]
	[JsonDerivedType(typeof(IdleStateDataRpcMessageRpcMessage), typeDiscriminator: "round")]
	public class BaseStateDataRpcMessage(string type)
	{
		public string Type { get; init; } = type; // hack for .NET 8, because SignalR not support JsonPolymorphic
	}

	public class IdleStateDataRpcMessageRpcMessage() : BaseStateDataRpcMessage("idle")
	{
		public required List<Guid> ReadyUsers { get; init; }
	}


	public class RoundStateDataRpcMessageRpcMessage() : BaseStateDataRpcMessage("round")
	{
		public required Dictionary<string, List<int>> Questions;

		public required Guid? Electioneer;
	}
}

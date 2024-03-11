using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnProfileChanged(ProfileAction action, ProfileDataRpcMessage? profile);


		Task OnLobbyChanged(LobbyAction action, LobbyDataRpcMessage? info);

		Task OnGameStateChanged(GameStateAction action, BaseStateData? info);

		Task OnServerError(string err);
	}

}

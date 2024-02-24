using DobirnaGraServer.Models.GameRPC;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnProfileChanged(ProfileAction action, ProfileData? profile);


		Task OnLobbyChanged(LobbyAction action, LobbyData? info);


		Task OnServerError(string err);
	}

}

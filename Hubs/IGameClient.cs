using DobirnaGraServer.Models.MessageTypes;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnProfileChanged(ProfileAction action, ProfileData? profile);


		Task OnLobbyChanged(LobbyAction action, LobbyInfo? info);


		Task OnServerError(string err);
	}

}

using DobirnaGraServer.Models.MessageTypes;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnProfileChanged(UserInfo info);


		Task OnLobbyChanged(LobbyAction action, LobbyInfo? info);


		Task OnServerError(string err);
	}

}

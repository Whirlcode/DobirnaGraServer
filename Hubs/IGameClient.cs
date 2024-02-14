using DobirnaGraServer.Models.MessageTypes;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnProfileChanged(UserInfo info);

		Task OnLobbyStateChanged(LobbyInfo info);

		Task OnServerError(string err);
	}

}

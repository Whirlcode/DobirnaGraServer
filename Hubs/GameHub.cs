using DobirnaGraServer.Models.RequestTypes;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnGameStateChanged(GameState state);
	}

	public class GameHub : Hub<IGameClient>
	{
		public Task CreateLobby(CreateLobbyRequest request)
		{
			return Clients.Caller.OnGameStateChanged(new GameState());
		}

		public Task JoinLobby(JoinLobbyRequest request)
		{
			return Clients.Caller.OnGameStateChanged(new GameState());
		}

		public Task TryTake()
		{
			return Clients.Caller.OnGameStateChanged(new GameState());
		}
	}
}

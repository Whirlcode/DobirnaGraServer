using DobirnaGraServer.Game;
using DobirnaGraServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class GameService(IHubContext<GameHub> hubContext)
	{
		private IHubContext<GameHub> HubContext { get; } = hubContext;

		private IDictionary<Guid, LobbyInstance> Lobbies = new Dictionary<Guid, LobbyInstance>();

		public void CreateLobby(UserInstance creator, string name)
		{
			LobbyInstance instance = new();
			instance.JoinUser(creator);
			Lobbies.Add(instance.Id, instance);
		}

		public void JoinLobby(UserInstance user, string inviteCode)
		{
			if (InviteCode.FindId(inviteCode, out Guid lobbyId) && Lobbies.TryGetValue(lobbyId, out LobbyInstance? lobby))
			{
				lobby.JoinUser(user);
			}
			else
			{
				throw new InvalidOperationException("Invalid invite code");
			}
		}

		public void LeaveLobby(UserInstance user)
		{
			user.CurrentLobby?.LeaveUser(user);
		}
	}
}

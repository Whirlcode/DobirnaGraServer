using DobirnaGraServer.Game;
using DobirnaGraServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class GameService(IHubContext<GameHub> hubContext)
	{
		private IHubContext<GameHub> HubContext { get; } = hubContext;

		private IDictionary<Guid, LobbyInstance> Lobbies = new Dictionary<Guid, LobbyInstance>();

		private readonly object _lockLobbies = new();

		public void CreateLobby(UserInstance creator, string name)
		{
			LobbyInstance instance = new();
			instance.JoinUser(creator);

			lock (_lockLobbies)
			{
				Lobbies.Add(instance.Id, instance);
			}

			instance.OnNumberUserChanged += OnNumberUserChanged;
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

		private void OnNumberUserChanged(LobbyInstance lobby)
		{
			if (lobby.NumberUser <= 0)
			{
				DestroyLobby(lobby);
			}
		}

		private void DestroyLobby(LobbyInstance lobby)
		{
			lobby.OnNumberUserChanged -= OnNumberUserChanged;

			lock (_lockLobbies)
			{
				Lobbies.Remove(lobby.Id);
			}

			lobby.Dispose();
		}
	}
}

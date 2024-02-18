using DobirnaGraServer.Game;
using DobirnaGraServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Services
{
	public class GameService(IHubContext<GameHub, IGameClient> hubContext)
	{
		private IDictionary<Guid, Lobby> Lobbies = new Dictionary<Guid, Lobby>();

		private readonly object _lockLobbies = new();

		public async Task CreateLobbyAsync(UserProfile creator, string name, int initialNumberPlaces, CancellationToken ct)
		{
			Lobby instance = new(hubContext, name, initialNumberPlaces);

			await instance.JoinUserAsMasterAsync(creator, ct);

			lock (_lockLobbies)
			{
				Lobbies.Add(instance.Id, instance);
			}

			instance.OnNumberUserChanged += OnNumberUserChanged;
		}

		public async Task JoinLobbyAsync(UserProfile user, string inviteCode, CancellationToken ct)
		{
			if (InviteCode.FindId(inviteCode, out Guid lobbyId) && Lobbies.TryGetValue(lobbyId, out Lobby? lobby))
			{
				await lobby.JoinUserAsync(user, ct);
			}
			else
			{
				throw new InvalidOperationException("Invalid invite code");
			}
		}

		private void OnNumberUserChanged(Lobby lobby)
		{
			if (!lobby.Users.Any())
			{
				DestroyLobby(lobby);
			}
		}

		private async void DestroyLobby(Lobby lobby)
		{
			lobby.OnNumberUserChanged -= OnNumberUserChanged;

			lock (_lockLobbies)
			{
				Lobbies.Remove(lobby.Id);
			}

			await lobby.DisposeAsync();
		}
	}
}

using DobirnaGraServer.Game;
using DobirnaGraServer.Models.RequestTypes;
using DobirnaGraServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnGameStateChanged(GameStateMessage state);
	}

	public class GameHub : Hub<IGameClient>
	{
		private readonly UserService _userService;

		public UserInstance Me
		{
			get => (UserInstance)Context.Items[nameof(UserInstance)]!;
			set => Context.Items.Add(nameof(UserInstance), value);
		}

		public GameHub(UserService userService, GameService gameService)
		{
			_userService = userService;
		}

		public override async Task OnConnectedAsync()
		{
			Me = await _userService.RegisterAsync(Context);

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await _userService.UnregisterAsync(Context);

			Context.Items.Clear();

			await base.OnDisconnectedAsync(exception);
		}

		public async Task CreateLobby(CreateLobbyActionMessage actionMessage, [FromServices] GameService game)
		{
			await game.CreateLobbyAsync(Me, actionMessage.Name);
		}

		public async Task JoinLobby(JoinLobbyActionMessage actionMessage, [FromServices] GameService game)
		{
			await game.JoinLobbyAsync(Me, actionMessage.InviteCode);
		}

		public Task TryTake()
		{
			return Task.CompletedTask;
		}
	}
}

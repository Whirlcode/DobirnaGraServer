using DobirnaGraServer.Game;
using DobirnaGraServer.Models.RequestTypes;
using DobirnaGraServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Hubs
{
	public interface IGameClient
	{
		Task OnGameStateChanged(GameState state);
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

		public override Task OnConnectedAsync()
		{
			Me = _userService.Register(Context);

			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
			_userService.Unregister(Context);

			Context.Items.Clear();

			return base.OnDisconnectedAsync(exception);
		}

		public Task CreateLobby(CreateLobbyRequest request, [FromServices] GameService game)
		{
			game.CreateLobby(Me, request.Name);

			return Task.CompletedTask;
		}

		public Task JoinLobby(JoinLobbyRequest request, [FromServices] GameService game)
		{
			game.JoinLobby(Me, request.InviteCode);

			return Task.CompletedTask;
		}

		public Task TryTake()
		{
			return Task.CompletedTask;
		}
	}
}

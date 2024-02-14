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

		Task OnServerError(string err);
	}

	public class GameHub : Hub<IGameClient>
	{
		private readonly UserService _userService;

		private readonly ILogger<GameHub> _logger;

		public UserInstance Me
		{
			get => (UserInstance)Context.Items[nameof(UserInstance)]!;
			set => Context.Items.Add(nameof(UserInstance), value);
		}

		public GameHub(UserService userService, GameService gameService, ILogger<GameHub> logger)
		{
			_userService = userService;
			_logger = logger;
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

		private Task HandleServerException(Exception e)
		{
			_logger.LogError(e, "Failed RPC");
			return Clients.Caller.OnServerError($"Failed RPC: {e.Message}\n\n Stack Trace:\n{e.StackTrace}");
		}

		public async Task CreateLobby(CreateLobbyActionMessage actionMessage, [FromServices] GameService game)
		{
			try
			{
				await game.CreateLobbyAsync(Me, actionMessage.Name, Context.ConnectionAborted);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
			
		}

		public async Task JoinLobby(JoinLobbyActionMessage actionMessage, [FromServices] GameService game)
		{
			try
			{
				await game.JoinLobbyAsync(Me, actionMessage.InviteCode);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public Task TryTake()
		{
			return Task.CompletedTask;
		}
	}
}

using DobirnaGraServer.Game;
using DobirnaGraServer.Models.RequestTypes;
using DobirnaGraServer.Services;
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

		public UserInstance Instance
		{
			get => (UserInstance)Context.Items[nameof(UserInstance)]!;
			set => Context.Items.Add(nameof(UserInstance), value);
		}

		public GameHub(UserService userService)
		{
			_userService = userService;
		}

		public override Task OnConnectedAsync()
		{
			Instance = _userService.Register(Context);

			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
			_userService.Unregister(Context);

			Context.Items.Clear();

			return base.OnDisconnectedAsync(exception);
		}

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

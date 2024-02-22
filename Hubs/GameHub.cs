using DobirnaGraServer.Game;
using DobirnaGraServer.Models.MessageTypes;
using DobirnaGraServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Hubs
{
	
	public class GameHub : Hub<IGameClient>
	{
		private readonly ProfileService _profileService;

		private readonly ILogger<GameHub> _logger;

		public UserProfile Me
		{
			get => (UserProfile)Context.Items[nameof(UserProfile)]!;
			set => Context.Items.Add(nameof(UserProfile), value);
		}

		public GameHub(ProfileService profileService, GameService gameService, ILogger<GameHub> logger)
		{
			_profileService = profileService;
			_logger = logger;
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();

			Me = await _profileService.RegisterAsync(Context);
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			await _profileService.UnregisterAsync(Me.Id);

			await base.OnDisconnectedAsync(exception);
		}

		private Task HandleServerException(Exception e)
		{
			_logger.LogError(e, "Failed RPC");
			return Clients.Caller.OnServerError($"Failed RPC: {e.Message}\n\n Stack Trace:\n{e.StackTrace}");
		}

		public async Task UpdateProfile(UpdateProfileActionMessage actionMessage)
		{
			try
			{
				if (actionMessage.Name != null)
				{
					Me.Name = actionMessage.Name;
				}
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task CreateLobby(CreateLobbyActionMessage actionMessage, [FromServices] GameService game)
		{
			try
			{
				Me.Name = actionMessage.UserName;

				await game.CreateLobbyAsync(Me, actionMessage.NameLobby, actionMessage.InitialNumberPlaces, Context.ConnectionAborted);
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
				Me.Name = actionMessage.UserName;

				await game.JoinLobbyAsync(Me, actionMessage.InviteCode, Context.ConnectionAborted);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task LeaveLobby([FromServices] GameService game)
		{
			try
			{
				if (Me.CurrentLobby != null)
				{
					await Me.CurrentLobby.LeaveUserAsync(Me, Context.ConnectionAborted);
				}
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task SetNumberPlaces(int number)
		{
			try
			{
				Me.CurrentLobby?.SetNumberPlaces(Me, number);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task RemovePlace(int index)
		{
			try
			{
				Me.CurrentLobby?.RemovePlace(Me, index);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task Seat(int index)
		{
			try
			{
				Me.CurrentLobby?.Seat(Me, index);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task SeatMaster()
		{
			try
			{
				Me.CurrentLobby?.SeatMaster(Me);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task Unseat()
		{
			try
			{
				Me.CurrentLobby?.Unseat(Me);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}

		public async Task ChangeScore(ChangeScoreActionMessage actionMessage)
		{
			try
			{
				Me.CurrentLobby?.ChangeScore(Me, actionMessage.TargetPlaceIndex, actionMessage.NewScore);
			}
			catch (Exception e)
			{
				await HandleServerException(e);
			}
		}
	}
}

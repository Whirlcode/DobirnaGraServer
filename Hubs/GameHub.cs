﻿using DobirnaGraServer.Game;
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
			await _profileService.UnregisterAsync(Context);

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
				await game.JoinLobbyAsync(Me, actionMessage.InviteCode, Context.ConnectionAborted);
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

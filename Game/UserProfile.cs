using DobirnaGraServer.Assets;
using DobirnaGraServer.Hubs;
using DobirnaGraServer.Models.GameRPC;
using Microsoft.AspNetCore.SignalR;

namespace DobirnaGraServer.Game
{
	public class UserProfile(IHubContext<GameHub, IGameClient> hubContext) : IProfile
	{
		public delegate void OnProfileChangedEvent();

		public event OnProfileChangedEvent OnProfileChanged = null!;

		private HubCallerContext? _userContext;

		public string ConnectionId => _userContext?.ConnectionId ?? string.Empty;

		public Guid Id { get; private init; } = Guid.NewGuid();

		private string _name = string.Empty;

		public string Name
		{
			get => _name;
			set
			{
				if (_name == value)
					return;
				_name = value;
				OnProfileChanged?.Invoke();
			}
		}

		private Lobby? _currentLobby;
		public Lobby? CurrentLobby
		{
			get => _currentLobby;
			set
			{
				if (_userContext == null)
					throw new InvalidOperationException("User is dead!");
				if (value != null && !value.HasUser(this))
					throw new InvalidOperationException("The user is not in the group.");
				if (value == null && _currentLobby != null && _currentLobby.HasUser(this))
					throw new InvalidOperationException("The user is still in the group.");

				_currentLobby = value;
			}
		}

		private ScopeFile? _avatar;
		public ScopeFile? Avatar
		{
			set
			{
				_avatar?.Dispose();
				_avatar = value;
				OnProfileChanged?.Invoke();
			}
			get => _avatar;
		}

		public async Task Login(HubCallerContext callerContext)
		{
			_userContext = callerContext;

			await OnLoggedIn();
		}

		public async Task Logout()
		{
			await OnLogout();

			if (CurrentLobby != null)
			{
				await CurrentLobby.LeaveUserAsync(this, default);
			}
			CurrentLobby = null;
			Avatar = null;
			_userContext = null;
		}

		private async Task OnLoggedIn()
		{
			await hubContext.Clients.Clients(ConnectionId).OnProfileChanged(ProfileAction.LoggedIn, ConvertToRpcData());

			OnProfileChanged += NotifyOnProfileChanged;
		}

		private async Task OnLogout()
		{
			OnProfileChanged -= NotifyOnProfileChanged;

			await hubContext.Clients.Clients(ConnectionId).OnProfileChanged(ProfileAction.Logout, null);
		}

		private ProfileData ConvertToRpcData()
		{
			return new ProfileData
			{
				Id = Id
			};
		}

		private async void NotifyOnProfileChanged()
		{
			await hubContext.Clients.Clients(ConnectionId)
				.OnProfileChanged(ProfileAction.Updated, ConvertToRpcData())
				.ConfigureAwait(false);
		}
	}
}

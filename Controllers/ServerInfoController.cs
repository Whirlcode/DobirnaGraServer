using DobirnaGraServer.Models;
using DobirnaGraServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DobirnaGraServer.Controllers
{
	[ApiController]
	[Route("/")]
	public class ServerInfoController : ControllerBase
	{
		[HttpGet]
		public ActionResult<ServerInfoModel> Get([FromServices] ProfileService profileService, [FromServices] GameService gameService)
		{
			return new ServerInfoModel
			{
				NumberUsers = profileService.NumberUsers,
				NumberLobbies = gameService.NumberLobbies,
			};
		}
	}
}

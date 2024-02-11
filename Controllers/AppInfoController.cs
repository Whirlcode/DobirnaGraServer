using DobirnaGraServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DobirnaGraServer.Controllers
{
	[ApiController]
	[Route("/")]
	public class AppInfoController : ControllerBase
	{
		private readonly ILogger<AppInfoController> _logger;

		public AppInfoController(ILogger<AppInfoController> logger)
		{
			_logger = logger;
		}

		[HttpGet(Name = "GetWeatherForecast")]
		public ActionResult<AppInfoModel> Get()
		{
			_logger.Log(LogLevel.Information, "Fetch App Info");
			return new AppInfoModel();
		}
	}
}

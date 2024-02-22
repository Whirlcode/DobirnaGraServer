using DobirnaGraServer.Assets;
using DobirnaGraServer.Game;
using DobirnaGraServer.Services;
using DobirnaGraServer.Utils;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;

namespace DobirnaGraServer.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[EnableCors()]
	public class AssetController : ControllerBase
	{
		[HttpPost("profile/upload")]
		public async Task<ActionResult> UploadProfileImage(
			[FromForm] Guid userId, 
			[FromForm] IFormFile file, 
			[FromServices] ProfileService profileService,
			[FromServices] ResourceService resourceService)
		{
			if (!ContentTypeExtensions.IsImageType(file.ContentType))
			{
				return BadRequest("Allowed only the content type: image/*");
			}

			if (!FilenameExtension.IsImage(file.FileName))
			{
				return BadRequest("The file extension does not match the image extension!");
			}

			int maxSizeFile = 25.Megabytes();
			if (file.Length > maxSizeFile)
			{
				return BadRequest($"The file size is greater than {maxSizeFile} bytes");
			}

			if (profileService.FindUser(userId, out UserProfile? user) && user != null)
			{
				await using Stream content = file.OpenReadStream();
				user.Avatar = await resourceService.SaveImageAsync(content, file.ContentType);

				return Ok($"{user.Avatar.Id}");
			}
			else
			{
				return NotFound($"Not found user: {userId.ToString()}");
			}
		}

		[HttpGet("profile/get/{imageId:long}")]
		public async Task<ActionResult> GetProfileImage(long imageId, [FromServices] ResourceService resourceService, CancellationToken cancellationToken)
		{
			if (resourceService.TryGetFile(imageId, out ScopeFile? file) && file != null)
			{
				byte[] image = await file.GetAllBytesAsync(cancellationToken);
				return File(image, file.Metadata.ContentType);
			}
			else
			{
				return NotFound($"Not found image: {imageId}");
			}
		}
	}
}

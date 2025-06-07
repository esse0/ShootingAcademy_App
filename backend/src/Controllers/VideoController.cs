using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Services;
using ShootingAcademy.Services.Media;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost("upload"), Authorize(Roles = "moderator")]
        public async Task<IActionResult> GenerateUploadUrl([FromBody] FileUploadRequest request)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _videoService.GeneratePresignedUploadAsync(request, userId);
            return Ok(result);
        }

        [HttpPost("confirm"), Authorize(Roles = "moderator")]
        public async Task<IActionResult> ConfirmUpload([FromBody] FileConfirmRequest request)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var media = await _videoService.ConfirmUploadAsync(request, userId);
            return Ok(media);
        }
    }
}

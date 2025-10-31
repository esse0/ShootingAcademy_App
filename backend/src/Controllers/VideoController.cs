using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Services;
using ShootingAcademy.Services.Media;

namespace ShootingAcademy.Controllers
{
    [Route("api/video")]
    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost("upload"), Authorize(Roles = "organization")]
        public async Task<IActionResult> GenerateUploadUrl([FromBody] FileUploadRequest request)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _videoService.GeneratePresignedUploadAsync(request, userId);
            return HandleResult(result);
        }

        [HttpPost("confirm"), Authorize(Roles = "organization")]
        public async Task<IActionResult> ConfirmUpload([FromQuery] FileConfirmRequest request)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var media = await _videoService.ConfirmUploadAsync(request, userId);

            return HandleResult(media.Id.ToString());
        }
    }
}

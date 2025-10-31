using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Services;
using ShootingAcademy.Services.Media;

namespace ShootingAcademy.Controllers
{
    [Route("api/image")]
    public class ImageController : BaseController
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload"), Authorize]
        public async Task<IActionResult> GenerateUploadUrl([FromBody] FileUploadRequest request)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var result = await _imageService.GeneratePresignedUploadAsync(request, userId);
            return HandleResult(result);
        }

        [HttpPost("confirm"), Authorize]
        public async Task<IActionResult> ConfirmUpload([FromQuery] FileConfirmRequest request)
        {
            var userId = AutorizeData.FromContext(HttpContext).UserGuid;
            var media = await _imageService.ConfirmUploadAsync(request, userId);
            await _imageService.ReplaceProfilePhoto(userId, media);
            return HandleResult();
        }
    }
}
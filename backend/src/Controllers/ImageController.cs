using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Models.Exceptions;
using ShootingAcademy.Services;
using ShootingAcademy.Services.Media;

namespace ShootingAcademy.Controllers
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload"), Authorize]
        public async Task<IResult> GenerateUploadUrl([FromBody] FileUploadRequest request)
        {
            try { 
                var userId = AutorizeData.FromContext(HttpContext).UserGuid;
                var result = await _imageService.GeneratePresignedUploadAsync(request, userId);

                return Results.Json(result);
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 400);
            }
        }

        [HttpPost("confirm"), Authorize]
        public async Task<IResult> ConfirmUpload([FromQuery] FileConfirmRequest request)
        {
            try
            {
                var userId = AutorizeData.FromContext(HttpContext).UserGuid;
                var media = await _imageService.ConfirmUploadAsync(request, userId);
                await _imageService.ReplaceProfilePhoto(userId, media);

                return Results.Ok();
            }
            catch (BaseException apperr)
            {
                return Results.Json(apperr.GetModel(), statusCode: apperr.Code);
            }
            catch (Exception err)
            {
                return Results.Problem(err.Message, statusCode: 400);
            }
        }
    }
}
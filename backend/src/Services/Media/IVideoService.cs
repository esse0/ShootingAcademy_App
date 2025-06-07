using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Models.DB;

namespace ShootingAcademy.Services.Media
{
    public interface IVideoService
    {
        Task<PresignedUrlResponse> GeneratePresignedUploadAsync(FileUploadRequest dto, Guid userId);
        Task<MediaStorage> ConfirmUploadAsync(FileConfirmRequest request, Guid userId);
    }
}

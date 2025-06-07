using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Models.DB;

namespace ShootingAcademy.Services.Media
{
    public interface IImageService
    {
        Task<PresignedUrlResponse> GeneratePresignedUploadAsync(FileUploadRequest dto, Guid userId);
        Task<MediaStorage> ConfirmUploadAsync(FileConfirmRequest request, Guid userId);
        Task<MediaStorage> ReplaceProfilePhoto(Guid confirmedByUserId, MediaStorage newMedia);
        Task<FileUriReponse> GetFileUrl(Guid userId);
        Task<bool> DeleteProfilePhoto(Guid userId);
    }
}

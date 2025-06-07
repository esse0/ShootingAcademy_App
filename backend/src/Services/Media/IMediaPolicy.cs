namespace ShootingAcademy.Services.Media
{
    public interface IMediaPolicy
    {
        bool IsMimeTypeAllowed(string mimeType);
        bool IsImage(string mimeType);
        bool IsVideo(string mimeType);
        string GetFolderByMimeType(string mimeType);
    }
}

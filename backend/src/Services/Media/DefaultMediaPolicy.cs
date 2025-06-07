namespace ShootingAcademy.Services.Media
{
    public class DefaultMediaPolicy : IMediaPolicy
    {
        private static readonly HashSet<string> AllowedImageMimeTypes = new()
        {
            "image/jpeg", "image/png"
        };

        private static readonly HashSet<string> AllowedVideoMimeTypes = new()
        {
            "video/mp4", "video/webm", "video/quicktime"
        };

        public bool IsMimeTypeAllowed(string mimeType) =>
            AllowedImageMimeTypes.Contains(mimeType) || AllowedVideoMimeTypes.Contains(mimeType);

        public bool IsImage(string mimeType) => AllowedImageMimeTypes.Contains(mimeType);

        public bool IsVideo(string mimeType) => AllowedVideoMimeTypes.Contains(mimeType);

        public string GetFolderByMimeType(string mimeType)
        {
            if (IsImage(mimeType)) return "images";
            if (IsVideo(mimeType)) return "videos";
            throw new InvalidOperationException("Unsupported MIME type.");
        }
    }
}

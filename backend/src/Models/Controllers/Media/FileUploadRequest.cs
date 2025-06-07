namespace ShootingAcademy.Models.Controllers.Media
{
    public class FileUploadRequest
    {
        public string FileName { get; set; } = null!;
        public string MimeType { get; set; } = null!;
    }
}

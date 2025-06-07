using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.DB
{
    public class PendingUpload
    {
        [Key]
        public Guid Id { get; set; }
        public string FileKey { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public Guid UploadedByUserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

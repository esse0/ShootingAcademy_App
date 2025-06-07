using ShootingAcademy.Models.DB.ModelUser;
using System.ComponentModel.DataAnnotations;

namespace ShootingAcademy.Models.DB
{
    public class MediaStorage
    {
        [Key]
        public Guid Id { get; set; }
        public string FileKey { get; set; } = null!;
        public long Size { get; set; }
        public string MimeType { get; set; } = null!;
        public DateTime UploadedAt { get; set; }

        public string? CachedUrl { get; set; }
        public DateTime? CachedUrlExpiresAt { get; set; }

        public Guid UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;

        public ProfilePhoto? ProfilePhoto { get; set; }
        public LessonVideo? LessonVideo { get; set; }
    }
}

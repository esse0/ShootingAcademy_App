using ShootingAcademy.Models.DB.ModelUser;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.DB
{
    public enum NotificationType
    {
        GroupInvitation,
        OrganizationInvitation,
        Message,
        System,
        TrainingReminder
    }

    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public Guid? SenderId { get; set; }
        [ForeignKey(nameof(SenderId))]
        public User? Sender { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public Guid? InviteId { get; set; }
        public bool RequiresResponse { get; set; }
        [StringLength(500)]
        public string? Response { get; set; }
        public DateTime? ResponseAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToUniversalTime();
    }
}

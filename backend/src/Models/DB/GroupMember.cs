using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.DB
{
    public enum GroupMemberStatus
    {
        Pending,        // Запрос на вступление отправлен, ожидает одобрения
        Approved,       // Принят в группу
        Rejected,       // Запрос отклонён
        Removed,        // Исключён из организации
        Left,           // Самостоятельно покинул организацию
        Banned          // Заблокирован
    }

    public class GroupMember
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid AthleteId { get; set; }
        [ForeignKey(nameof(AthleteId))]
        public User Athlete { get; set; }

        [Required]
        public Guid AthleteGroupId { get; set; }
        [ForeignKey(nameof(AthleteGroupId))]
        public AthleteGroup AthleteGroup { get; set; }

        public GroupMemberStatus Status { get; set; }
    }
}

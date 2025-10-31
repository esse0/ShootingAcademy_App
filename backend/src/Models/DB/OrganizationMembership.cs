using ShootingAcademy.Models.DB.ModelUser;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.DB
{
    public enum OrganizationMemberStatus
    {
        Pending,        // Запрос на вступление отправлен, ожидает одобрения
        Approved,       // Принят в организацию
        Rejected,       // Запрос отклонён
        Removed,        // Исключён из организации
        Left,           // Самостоятельно покинул организацию
        Banned          // Заблокирован
    }

    public class OrganizationMembership
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public Guid OrganizationId { get; set; }
        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; }

        public string Role {  get; set; }
        public OrganizationMemberStatus Status { get; set; }
        public DateTime JoinedAt {  get; set; }
    }
}

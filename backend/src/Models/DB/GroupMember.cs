using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.DB
{
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
    }
}

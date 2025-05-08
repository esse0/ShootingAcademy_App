using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.DB
{
    public class AthleteGroup
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CoachId { get; set; }

        public string OrganisationName { get; set; }

        [ForeignKey(nameof(CoachId))]
        public User Coach { get; set; }

        public List<GroupMember> Athletes { get; set; } = [];
    }
}

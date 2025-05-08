using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShootingAcademy.Models.DB.ModelUser;

namespace ShootingAcademy.Models.DB
{
    public class Competition
    {
        public enum ActiveStatus
        {
            Pending, Active, Ended
        }


        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public int MaxMembersCount { get; set; }

        public string Venue { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public string Exercise { get; set; }

        [Required]
        public Guid OrganisationId { get; set; }
        [ForeignKey(nameof(OrganisationId))]
        public User Organisation { get; set; }

        public ActiveStatus Status { get; set; }

        public List<CompetitionMember> Members { get; set; } = [];
    }
}

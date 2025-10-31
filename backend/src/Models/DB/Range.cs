using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.DB
{
    public enum RangeType
    {
        Indoor,
        Outdoor,
        Virtual,
        Other
    }

    public class Range
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; }
        public string Location { get; set; }
        public string Description {  get; set; }
        public int Capacity { get; set; }
        public RangeType Type { get; set; }
        public bool IsActive { get; set; }

        public List<TrainingSession> TrainingSessions { get; set; }
    }
}

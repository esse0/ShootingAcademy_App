using ShootingAcademy.Models.DB.ModelUser;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShootingAcademy.Models.DB
{
    public enum TrainingSessionStatus
    {
        Planned,
        Ongoing,
        Completed,
        Cancelled
    }

    public class TrainingSession
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        [ForeignKey(nameof(TrainerId))]
        public User Trainer { get; set; }

        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public AthleteGroup Group { get; set; }
        public Guid RangeId { get; set; }
        [ForeignKey(nameof(RangeId))]
        public Range Range { get; set; }
        public TrainingSessionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

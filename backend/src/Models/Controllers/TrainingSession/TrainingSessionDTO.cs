using System;

namespace ShootingAcademy.Models.Controllers.TrainingSession
{
    public class TrainingSessionDTO
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public string TrainerName { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public Guid RangeId { get; set; }
        public string RangeName { get; set; }
        public string RangeLocation { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CreateTrainingSessionDTO
    {
        public Guid GroupId { get; set; }
        public Guid RangeId { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UpdateTrainingSessionDTO
    {
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetScheduleRequestDTO
    {
        public Guid? CoachId { get; set; }
        public Guid? AthleteId { get; set; }
    }

    public class GroupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class RangeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
} 
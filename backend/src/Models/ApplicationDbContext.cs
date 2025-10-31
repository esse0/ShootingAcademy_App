using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;
using Range = ShootingAcademy.Models.DB.Range;

namespace ShootingAcademy.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<AthleteGroup> AthleteGroups { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<CompetitionMember> CompetitionMembers { get; set; }
        public DbSet<CourseMember> CourseMembers { get; set; }
        public DbSet<MediaStorage> MediaStorages { get; set; }
        public DbSet<ProfilePhoto> ProfilePhotos { get; set; }
        public DbSet<LessonVideo> LessonVideos { get; set; }
        public DbSet<PendingUpload> PendingUploads { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationMembership> OrganizationMemberships { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<Range> Ranges { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Настройка связей для LessonVideo
            modelBuilder.Entity<LessonVideo>()
                .HasOne(lv => lv.Lesson)
                .WithOne(l => l.LessonVideo)
                .HasForeignKey<LessonVideo>(lv => lv.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LessonVideo>()
                .HasOne(lv => lv.Media)
                .WithOne(m => m.LessonVideo)
                .HasForeignKey<LessonVideo>(lv => lv.MediaId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}

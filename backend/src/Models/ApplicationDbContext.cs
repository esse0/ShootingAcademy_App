using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Models.DB.ModelUser;

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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}

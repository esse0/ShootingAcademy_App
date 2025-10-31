using ShootingAcademy.Models.DB.ModelUser;
using ShootingAcademy.Models;
using Microsoft.EntityFrameworkCore;

namespace ShootingAcademy.Services
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();

            context.Database.EnsureCreated();

            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@gmail.com";
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "123password";

            if (!context.Users.Any(u => u.Email.ToLower() == adminEmail.ToLower()))
            {
                var admin = new User
                {
                    Email = adminEmail,
                    Role = "admin",
                    FirstName = "admin",
                    SecoundName = "admin",
                    PatronymicName = string.Empty,
                    PasswordHash = passwordHasher.Hash(adminPassword),
                    Grade = "",
                    Age = 0,
                    Country = "",
                    City = "",
                    Address = "",
                    RToken = ""
                };

                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
    }
}

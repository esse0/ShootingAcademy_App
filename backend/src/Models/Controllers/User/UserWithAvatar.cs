namespace ShootingAcademy.Models.DB.ModelUser
{
    public class UserWithAvatar
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string SecoundName { get; set; }
        public string PatronymicName { get; set; }

        public int Age { get; set; }

        public string Grade { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }
        public string ProfilePhotoUri { get; set; }

        public static UserWithAvatar FromEntity(User user, string profilePhoroUri)
        {
            return new UserWithAvatar
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SecoundName = user.SecoundName,
                Address = user.Address,
                Age = user.Age,
                Grade = user.Grade,
                City = user.City,
                Country = user.Country,
                Email = user.Email,
                PatronymicName = user.PatronymicName,
                Role = user.Role,
                ProfilePhotoUri = profilePhoroUri
            };
        }
    }
}

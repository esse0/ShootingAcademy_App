using System.Security.Cryptography;
using System.Text;

namespace ShootingAcademy.Services
{
    public class PasswordHasher
    {
        private readonly string _salt;

        public PasswordHasher(string salt)
        {
            _salt = salt;
        }

        public string Hash(string password)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(_salt);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var hasher = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = hasher.GetBytes(32);

                return Convert.ToBase64String(hash);
            }
        }

        public bool Verify(string password, string hash)
        {
            return Hash(password) == hash;
        }
    }
}

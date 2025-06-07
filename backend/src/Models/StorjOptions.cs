namespace ShootingAcademy.Models
{
    public class StorjOptions
    {
        public string BucketName { get; set; } = null!;
        public string Endpoint { get; set; } = null!;
        public int PresignedExpiryMinutes { get; set; } = 30;
    }
}

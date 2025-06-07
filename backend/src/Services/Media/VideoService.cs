using Amazon.S3;
using Microsoft.Extensions.Options;
using ShootingAcademy.Models;
using ShootingAcademy.Services.media;

namespace ShootingAcademy.Services.Media
{
    public class VideoService : BaseMediaService, IVideoService
    {
        public VideoService(IAmazonS3 s3, ApplicationDbContext db, IOptions<StorjOptions> options, IMediaPolicy policy)
            : base(s3, db, options, policy) { }
    }
}

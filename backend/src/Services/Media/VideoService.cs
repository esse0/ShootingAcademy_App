using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Services.media;

namespace ShootingAcademy.Services.Media
{
    public class VideoService : BaseMediaService, IVideoService
    {
        public VideoService(IAmazonS3 s3, ApplicationDbContext db, IOptions<StorjOptions> options, IMediaPolicy policy)
            : base(s3, db, options, policy) { }

        public async Task<FileUriReponse> GetFileUrl(Guid videoId)
        {
            var media = await Context.MediaStorages.FirstOrDefaultAsync(el => el.Id == videoId);

            if (media == null)
                return new FileUriReponse()
                {
                    FileUri = ""
                };

            if (!string.IsNullOrEmpty(media.CachedUrl) && media.CachedUrlExpiresAt?.ToUniversalTime() > DateTime.UtcNow.ToUniversalTime())
            {
                return new FileUriReponse()
                {
                    FileUri = media.CachedUrl,
                };
            }

            var expiresInDays = 1;

            var request = new GetPreSignedUrlRequest
            {
                BucketName = Config.BucketName,
                Key = media.FileKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(expiresInDays)
            };

            var newUrl = AmazonS3.GetPreSignedURL(request);

            media.CachedUrl = newUrl;
            media.CachedUrlExpiresAt = DateTime.UtcNow.AddDays(expiresInDays).ToUniversalTime();

            Context.MediaStorages.Update(media);
            await Context.SaveChangesAsync();

            return new FileUriReponse() { FileUri = newUrl };
        }
    }
}

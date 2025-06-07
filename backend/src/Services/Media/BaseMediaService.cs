using Amazon.S3;
using Amazon.S3.Model;
using ShootingAcademy.Models;
using Microsoft.Extensions.Options;
using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Models.DB;
using Microsoft.EntityFrameworkCore;
using ShootingAcademy.Services.Media;
using System.Net;


namespace ShootingAcademy.Services.media
{
    public abstract class BaseMediaService
    {
        protected readonly IAmazonS3 AmazonS3;
        protected readonly ApplicationDbContext Context;
        protected readonly StorjOptions Config;
        protected readonly IMediaPolicy MediaPolicy;

        protected BaseMediaService(IAmazonS3 s3Client, ApplicationDbContext context, IOptions<StorjOptions> options, IMediaPolicy mediaPolicy)
        {
            AmazonS3 = s3Client;
            Context = context;
            Config = options.Value;
            MediaPolicy = mediaPolicy;
        }

        public virtual async Task<PresignedUrlResponse> GeneratePresignedUploadAsync(FileUploadRequest dto, Guid userId)
        {
            var folder = MediaPolicy.GetFolderByMimeType(dto.MimeType);
            var key = $"{folder}/{Guid.NewGuid()}_{dto.FileName}";

            var request = new GetPreSignedUrlRequest
            {
                BucketName = Config.BucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(Config.PresignedExpiryMinutes),
                ContentType = dto.MimeType
            };

            var presignedUrl = AmazonS3.GetPreSignedURL(request);

            var pendingUpload = new PendingUpload
            {
                Id = Guid.NewGuid(),
                MimeType = dto.MimeType,
                UploadedByUserId = userId,
                FileKey = key,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Config.PresignedExpiryMinutes + 5).ToUniversalTime()
            };

            Context.PendingUploads.Add(pendingUpload);

            await Context.SaveChangesAsync();

            return new PresignedUrlResponse
            {
                TempFileId = pendingUpload.Id.ToString(),
                UploadUrl = presignedUrl
            };
        }

        public virtual async Task<MediaStorage> ConfirmUploadAsync(FileConfirmRequest request, Guid confirmedByUserId)
        {
            if (string.IsNullOrWhiteSpace(request.FileId))
                throw new ArgumentException("File ID is required.");

            if (!Guid.TryParse(request.FileId, out Guid fileGuid))
                throw new InvalidOperationException("Invalid file ID format.");

            var pending = await Context.PendingUploads
                .FirstOrDefaultAsync(el => el.Id == fileGuid);

            if (pending == null || pending.ExpiresAt.ToUniversalTime() < DateTime.UtcNow.ToUniversalTime())
                throw new InvalidOperationException("Upload confirmation expired or invalid.");

            if (pending.UploadedByUserId != confirmedByUserId)
                throw new UnauthorizedAccessException("You cannot confirm someone else's upload.");

            var key = pending.FileKey;

            GetObjectMetadataResponse metadata;
            try
            {
                metadata = await AmazonS3.GetObjectMetadataAsync(new GetObjectMetadataRequest
                {
                    BucketName = Config.BucketName,
                    Key = key
                });
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException("File not found.", ex);
            }
            catch (AmazonS3Exception ex)
            {
                throw new InvalidOperationException("S3 - Error.", ex);
            }

            var media = new MediaStorage
            {
                Id = Guid.NewGuid(),
                MimeType = pending.MimeType,
                Size = metadata.ContentLength,
                UploadedAt = DateTime.UtcNow.ToUniversalTime(),
                UploadedByUserId = confirmedByUserId,
                FileKey = key
            };

            Context.PendingUploads.Remove(pending);
            Context.MediaStorages.Add(media);

            await Context.SaveChangesAsync();

            return media;
        }
    }
}

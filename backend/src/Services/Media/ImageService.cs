using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShootingAcademy.Models;
using ShootingAcademy.Models.Controllers.Media;
using ShootingAcademy.Models.DB;
using ShootingAcademy.Services.media;
using System.Net;

namespace ShootingAcademy.Services.Media
{
    public class ImageService : BaseMediaService, IImageService
    {
        public ImageService(IAmazonS3 s3, ApplicationDbContext context, IOptions<StorjOptions> options, IMediaPolicy policy)
            : base(s3, context, options, policy) { }

        public async Task<MediaStorage> ReplaceProfilePhoto(Guid confirmedByUserId, MediaStorage newMedia)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
           
            try
            {
                var user = await Context.Users
                    .Include(u => u.ProfilePhoto)
                        .ThenInclude(p => p.Media)
                    .FirstOrDefaultAsync(u => u.Id == confirmedByUserId) ?? throw new InvalidOperationException("User not found");

                if (user.ProfilePhoto != null)
                {
                    var oldMedia = user.ProfilePhoto.Media;

                    if (oldMedia != null)
                    {
                        try
                        {
                            await AmazonS3.DeleteObjectAsync(new DeleteObjectRequest
                            {
                                BucketName = Config.BucketName,
                                Key = oldMedia.FileKey
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

                        Context.MediaStorages.Remove(oldMedia);
                    }

                    user.ProfilePhoto.MediaId = newMedia.Id;
                    user.ProfilePhoto.Media = newMedia;
                    Context.ProfilePhotos.Update(user.ProfilePhoto);
                }
                else
                {
                    var newProfilePhoto = new ProfilePhoto
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        MediaId = newMedia.Id,
                        Media = newMedia
                    };

                    Context.ProfilePhotos.Add(newProfilePhoto);
                    user.ProfilePhoto = newProfilePhoto;
                }

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                return newMedia;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<FileUriReponse> GetFileUrl(Guid userId)
        {
            var user = await Context.Users
                .Include(u => u.ProfilePhoto)
                    .ThenInclude(p => p.Media)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException("User not found");

            var photo = user.ProfilePhoto;
            var media = photo?.Media;

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

        public async Task<bool> DeleteProfilePhoto(Guid userId)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                var user = await Context.Users
                    .Include(u => u.ProfilePhoto)
                        .ThenInclude(p => p.Media)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    throw new InvalidOperationException("User not found");

                if (user.ProfilePhoto != null)
                {
                    var oldMedia = user.ProfilePhoto.Media;

                    if (oldMedia != null)
                    {
                        try
                        {
                            await AmazonS3.DeleteObjectAsync(new DeleteObjectRequest
                            {
                                BucketName = Config.BucketName,
                                Key = oldMedia.FileKey
                            });
                        }
                        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                        {
                            
                        }
                        catch (AmazonS3Exception ex)
                        {
                            throw new InvalidOperationException("Ошибка при удалении из S3.", ex);
                        }

                        Context.MediaStorages.Remove(oldMedia);
                    }

                    Context.ProfilePhotos.Remove(user.ProfilePhoto);
                    user.ProfilePhoto = null;
                }

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}

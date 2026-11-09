using System;
using System.Threading.Tasks;
using API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private Cloudinary _cloudinary;
        private readonly bool _isConfigured;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var settings = config.Value;
            _isConfigured = IsConfigured(settings?.CloudName) &&
                            IsConfigured(settings?.ApiKey) &&
                            IsConfigured(settings?.ApiSecret);
            if (_isConfigured)
            {
                var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                _cloudinary = new Cloudinary(account);
            }
        }

        public async Task<ImageUploadResult> UploadImage(IFormFile formFile, bool profile)
        {
            EnsureConfigured();
            var uploadResult = new ImageUploadResult();
            var transformation = profile
                ? new Transformation().Width(500).Height(500).Crop("fill").Gravity(Gravity.Face)
                : new Transformation().Height(512).Crop("fit");
            if (formFile.Length > 0)
            {
                await using var stream = formFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Transformation = transformation,
                    Folder = "ShoppingCart"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrEmpty(publicId)) return null;
            EnsureConfigured();
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }

        private static bool IsConfigured(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   !value.StartsWith("<ADD_", StringComparison.Ordinal);
        }

        private void EnsureConfigured()
        {
            if (!_isConfigured)
                throw new HttpException(
                    "Photo uploads require valid Cloudinary credentials. Set CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY, and CLOUDINARY_API_SECRET in the root .env file.",
                    StatusCodes.Status503ServiceUnavailable);
        }
    }
}

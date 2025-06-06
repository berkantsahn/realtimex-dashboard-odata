using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Xabe.FFmpeg;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.Services
{
    public class MediaService : IMediaService
    {
        private readonly string _mediaStoragePath;
        private readonly string _thumbnailStoragePath;

        public MediaService(IConfiguration configuration)
        {
            _mediaStoragePath = configuration["MediaStorage:Path"] ?? "wwwroot/media";
            _thumbnailStoragePath = Path.Combine(_mediaStoragePath, "thumbnails");
            
            // Dizinleri oluştur
            Directory.CreateDirectory(_mediaStoragePath);
            Directory.CreateDirectory(_thumbnailStoragePath);
        }

        public async Task<MediaUploadResult> UploadMediaAsync(IFormFile file, string userId)
        {
            var fileName = $"{userId}_{DateTime.UtcNow.Ticks}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_mediaStoragePath, fileName);
            var metadata = await GetMediaMetadataAsync(file);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var thumbnailUrl = await GenerateThumbnailAsync(file, GetMediaType(file.ContentType));

            return new MediaUploadResult
            {
                Url = $"/media/{fileName}",
                ThumbnailUrl = thumbnailUrl,
                Size = metadata.Size,
                Name = fileName,
                MimeType = metadata.MimeType,
                Duration = metadata.Duration
            };
        }

        public async Task<bool> DeleteMediaAsync(string mediaUrl)
        {
            var fileName = Path.GetFileName(mediaUrl);
            var filePath = Path.Combine(_mediaStoragePath, fileName);
            var thumbnailPath = Path.Combine(_thumbnailStoragePath, $"thumb_{fileName}.jpg");

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                if (File.Exists(thumbnailPath))
                    File.Delete(thumbnailPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<byte[]> GetMediaAsync(string mediaUrl)
        {
            var fileName = Path.GetFileName(mediaUrl);
            var filePath = Path.Combine(_mediaStoragePath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task<string> GenerateThumbnailAsync(IFormFile file, MediaType mediaType)
        {
            var fileName = $"thumb_{DateTime.UtcNow.Ticks}_{Path.GetFileNameWithoutExtension(file.FileName)}.jpg";
            var thumbnailPath = Path.Combine(_thumbnailStoragePath, fileName);

            switch (mediaType)
            {
                case MediaType.Image:
                    await GenerateImageThumbnailAsync(file, thumbnailPath);
                    break;

                case MediaType.Video:
                    await GenerateVideoThumbnailAsync(file, thumbnailPath);
                    break;

                case MediaType.Audio:
                    // Ses dosyaları için varsayılan bir ikon kullan
                    File.Copy("wwwroot/images/audio-icon.png", thumbnailPath);
                    break;

                case MediaType.Document:
                    // Dökümanlar için varsayılan bir ikon kullan
                    File.Copy("wwwroot/images/document-icon.png", thumbnailPath);
                    break;
            }

            return $"/media/thumbnails/{fileName}";
        }

        public async Task<MediaMetadata> GetMediaMetadataAsync(IFormFile file)
        {
            var metadata = new MediaMetadata
            {
                Size = file.Length,
                MimeType = file.ContentType
            };

            var mediaType = GetMediaType(file.ContentType);

            switch (mediaType)
            {
                case MediaType.Video:
                case MediaType.Audio:
                    using (var tempFile = Path.GetTempFileName())
                    {
                        using (var stream = new FileStream(tempFile, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var mediaInfo = await FFmpeg.GetMediaInfo(tempFile);
                        metadata.Duration = (int)mediaInfo.Duration.TotalSeconds;

                        if (mediaType == MediaType.Video)
                        {
                            var videoStream = mediaInfo.VideoStreams.First();
                            metadata.Width = videoStream.Width;
                            metadata.Height = videoStream.Height;
                        }

                        File.Delete(tempFile);
                    }
                    break;

                case MediaType.Image:
                    using (var stream = file.OpenReadStream())
                    using (var image = await Image.LoadAsync(stream))
                    {
                        metadata.Width = image.Width;
                        metadata.Height = image.Height;
                    }
                    break;
            }

            return metadata;
        }

        private async Task GenerateImageThumbnailAsync(IFormFile file, string outputPath)
        {
            using (var stream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(stream))
            {
                var maxDimension = 200;
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(maxDimension, maxDimension),
                    Mode = ResizeMode.Max
                }));
                await image.SaveAsJpegAsync(outputPath);
            }
        }

        private async Task GenerateVideoThumbnailAsync(IFormFile file, string outputPath)
        {
            using (var tempFile = Path.GetTempFileName())
            {
                using (var stream = new FileStream(tempFile, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var mediaInfo = await FFmpeg.GetMediaInfo(tempFile);
                var duration = mediaInfo.Duration.TotalSeconds;
                
                // Video'nun ortasından bir kare al
                var snapshot = await FFmpeg.Conversions.New()
                    .AddParameter($"-ss {duration / 2}")
                    .AddParameter("-frames:v 1")
                    .SetInputFile(tempFile)
                    .SetOutputFile(outputPath)
                    .Start();

                File.Delete(tempFile);
            }
        }

        private MediaType GetMediaType(string contentType)
        {
            if (contentType.StartsWith("image/"))
                return MediaType.Image;
            if (contentType.StartsWith("video/"))
                return MediaType.Video;
            if (contentType.StartsWith("audio/"))
                return MediaType.Audio;
            return MediaType.Document;
        }
    }
} 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RealtimeX.Dashboard.Core.Enums;
using System;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IMediaService
    {
        Task<MediaUploadResult> UploadMediaAsync(IFormFile file, string userId);
        Task<bool> DeleteMediaAsync(string mediaUrl);
        Task<byte[]> GetMediaAsync(string mediaUrl);
        Task<string> GenerateThumbnailAsync(IFormFile file, MediaType mediaType);
        Task<MediaMetadata> GetMediaMetadataAsync(IFormFile file);
    }

    public class MediaUploadResult
    {
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public TimeSpan? Duration { get; set; }
    }

    public class MediaMetadata
    {
        public long Size { get; set; }
        public string MimeType { get; set; }
        public TimeSpan? Duration { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
} 
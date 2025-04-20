using Soundy.SharedLibrary.Common.Response;

namespace Soundy.CatalogService.Interfaces
{
    public interface IObjectStorage
    {
        Task<Response<bool>> ObjectExistsAsync(string key, CancellationToken ct = default);
        Task<Response<string>> GeneratePresignedUrlAsync(string key, string contentType, TimeSpan expiresIn, CancellationToken ct = default);
        Task<Response> DeleteObjectAsync(string key, CancellationToken ct = default);
    }
}

using Grpc.Core;
using Service.File;

namespace Soundy.FileService.Interfaces
{
    public interface ITrackFileService
    {
        Task StreamTrackAsync(string trackId, string fileName, Func<byte[], string, Task> onChunk, CancellationToken ct);
        Task<UploadTrackResponse> UploadTrack(IAsyncStreamReader<UploadTrackRequest?> requestStream, CancellationToken ct = default);
        Task<UploadImageResponse> UploadImage(UploadImageRequest request, CancellationToken ct = default);
        Task<DownloadImageResponse> DownloadImage(DownloadImageRequest request, CancellationToken ct = default);
    }
}

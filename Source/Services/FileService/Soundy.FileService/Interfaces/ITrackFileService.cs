namespace Soundy.FileService.Interfaces
{
    public interface ITrackFileService
    {
        Task<string> UploadTrackAsync(string trackId, Stream fileStream, CancellationToken ct = default);
        Task<Stream> DownloadTrackAsync(string trackId, CancellationToken ct = default);
    }
}

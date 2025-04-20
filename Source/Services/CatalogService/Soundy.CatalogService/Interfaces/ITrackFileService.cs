namespace Soundy.CatalogService.Interfaces
{
    public interface ITrackFileService
    {
        Task UploadTrackAsync(string trackId, Stream fileStream, CancellationToken ct = default);
        Task<Stream> DownloadTrackAsync(string trackId, CancellationToken ct = default);
    }
}

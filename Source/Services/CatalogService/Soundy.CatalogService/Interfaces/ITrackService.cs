using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.Interfaces
{
    public interface ITrackMetadataService
    {
        Task<Track> CreateTrackAsync(CreateTrackRequest request, CancellationToken ct = default);
        Task<Track> GetTrackAsync(string id, CancellationToken ct = default);
        Task<Track> UpdateTrackAsync(UpdateTrackRequest request, CancellationToken ct = default);
        Task<bool> DeleteTrackAsync(string id, CancellationToken ct = default);
        IAsyncEnumerable<Track> ListTracksAsync(int page, int pageSize, CancellationToken ct = default);
        IAsyncEnumerable<Track> SearchTracksAsync(string query, CancellationToken ct = default);
        IAsyncEnumerable<Track> GetTracksByPlaylistAsync(string playlistId, CancellationToken ct = default);
        IAsyncEnumerable<Track> GetTracksByUserAsync(string userId, CancellationToken ct = default);
    }
}

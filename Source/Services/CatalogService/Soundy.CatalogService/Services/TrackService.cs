using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Services
{
    public class TrackService : ITrackMetadataService
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<TrackService> _logger;

        public TrackService(
            DatabaseContext dbContext,
            ILogger<TrackService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Track> CreateTrackAsync(CreateTrackRequest request, CancellationToken ct = default)
        {
            var track = new Track
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                UserId = Guid.Parse(request.UserId),
                PlaylistId = Guid.Parse(request.PlaylistId),
                Duration = request.Duration,
                UploadDate = DateTime.UtcNow
            };

            await _dbContext.Tracks.AddAsync(track, ct);
            await _dbContext.SaveChangesAsync(ct);

            return track;
        }

        public async Task<Track> GetTrackAsync(string id, CancellationToken ct = default)
        {
            if (!Guid.TryParse(id, out var trackId))
                throw new ArgumentException("Invalid track ID format");

            var track = await _dbContext.Tracks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == trackId, ct);

            return track ?? throw new KeyNotFoundException($"Track {id} not found");
        }

        public async Task<Track> UpdateTrackAsync(UpdateTrackRequest request, CancellationToken ct = default)
        {
            if (!Guid.TryParse(request.Id, out var trackId))
                throw new ArgumentException("Invalid track ID format");

            var track = await _dbContext.Tracks
                .FirstOrDefaultAsync(t => t.Id == trackId, ct)
                ?? throw new KeyNotFoundException($"Track {request.Id} not found");

            if (!string.IsNullOrEmpty(request.Title))
                track.Title = request.Title;

            if (!string.IsNullOrEmpty(request.PlaylistId))
                track.PlaylistId = Guid.Parse(request.PlaylistId);

            await _dbContext.SaveChangesAsync(ct);
            return track;
        }

        public async Task<bool> DeleteTrackAsync(string id, CancellationToken ct = default)
        {
            if (!Guid.TryParse(id, out var trackId))
                throw new ArgumentException("Invalid track ID format");

            var track = await _dbContext.Tracks
                .FirstOrDefaultAsync(t => t.Id == trackId, ct);

            if (track == null) return false;

            _dbContext.Tracks.Remove(track);
            await _dbContext.SaveChangesAsync(ct);
            return true;
        }

        public async IAsyncEnumerable<Track> ListTracksAsync(int page, int pageSize, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var query = _dbContext.Tracks
                .AsNoTracking()
                .OrderBy(t => t.UploadDate)
                .Skip(page * pageSize)
                .Take(pageSize);

            await foreach (var track in query.AsAsyncEnumerable().WithCancellation(ct))
            {
                yield return track;
            }
        }

        public async IAsyncEnumerable<Track> SearchTracksAsync(string query, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var searchQuery = _dbContext.Tracks
                .AsNoTracking()
                .Where(t => EF.Functions.ILike(t.Title, $"%{query}%"))
                .OrderBy(t => t.Title);

            await foreach (var track in searchQuery.AsAsyncEnumerable().WithCancellation(ct))
            {
                yield return track;
            }
        }

        public async IAsyncEnumerable<Track> GetTracksByPlaylistAsync(string playlistId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (!Guid.TryParse(playlistId, out var playlistGuid))
                throw new ArgumentException("Invalid playlist ID format");

            var query = _dbContext.Tracks
                .AsNoTracking()
                .Where(t => t.PlaylistId == playlistGuid)
                .OrderBy(t => t.UploadDate);

            await foreach (var track in query.AsAsyncEnumerable().WithCancellation(ct))
            {
                yield return track;
            }
        }

        public async IAsyncEnumerable<Track> GetTracksByUserAsync(string userId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new ArgumentException("Invalid user ID format");

            var query = _dbContext.Tracks
                .AsNoTracking()
                .Where(t => t.UserId == userGuid)
                .OrderByDescending(t => t.UploadDate);

            await foreach (var track in query.AsAsyncEnumerable().WithCancellation(ct))
            {
                yield return track;
            }
        }
    }
}

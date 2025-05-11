using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.CatalogService.DataAccess;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.AlbumDtos;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;
        private readonly ILogger<TrackService> _logger;
        private readonly IMapper _mapper;

        public AlbumService(IDbContextFactory<DatabaseContext> dbFactory, ILogger<TrackService> logger, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CreateResponseDto> CreateAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
            var album = new Album()
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow,
                AvatarUrl = dto.AvatarUrl
            };

            await dbContext.Albums.AddAsync(album, ct);
            await dbContext.SaveChangesAsync(ct);
            return new CreateResponseDto()
            {
                Album = _mapper.Map<AlbumDto>(album)
            };
        }

        public async Task<AddTrackResponseDto> AddTrackAsync(AddTrackRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var album = await dbContext.Albums.Include(album => album.Tracks).FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
            var track = await dbContext.Tracks.FirstOrDefaultAsync(x => x.Id == dto.TrackId, ct);
            if (track == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            
            album?.Tracks.Add(track);

            await dbContext.SaveChangesAsync(ct);
            return new AddTrackResponseDto()
            {
                Album = _mapper.Map<AlbumDto>(album)
            };
        }
    }
}

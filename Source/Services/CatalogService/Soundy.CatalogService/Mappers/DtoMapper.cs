using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.Mappers
{
    public class DtoMapper : Profile
    {
        public DtoMapper()
        {
            CreateMap<PlaylistTrack, PlaylistTrackDto>()
                .ForMember(dest => dest.Track, opt => opt.MapFrom(src => src.Track))
                .ForMember(dest => dest.TrackId, opt => opt.MapFrom(src => src.TrackId))
                .ForMember(dest => dest.Playlist, opt => opt.MapFrom(src => src.Playlist))
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => src.PlaylistId))
                .ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.AddedDate));

            CreateMap<PlaylistTrackDto, PlaylistTrack>()
                .ForMember(dest => dest.Track, opt => opt.MapFrom(src => src.Track))
                .ForMember(dest => dest.TrackId, opt => opt.MapFrom(src => src.TrackId))
                .ForMember(dest => dest.Playlist, opt => opt.MapFrom(src => src.Playlist))
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => src.PlaylistId))
                .ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.AddedDate));

            CreateMap<PlaylistTrackDto, TrackDto>()
                .ConvertUsing(src => new TrackDto
                {
                    Id = src.Track.Id,
                    AuthorId = src.Track.AuthorId,
                    AlbumId = src.Track.AlbumId,
                    Title = src.Track.Title,
                    CreatedAt = src.Track.CreatedAt,
                    Duration = src.Track.Duration,
                    AvatarUrl = src.Track.AvatarUrl
                });

            CreateMap<PlaylistTrack, TrackDto>()
                .ConvertUsing(src => new TrackDto
                {
                    Id = src.Track.Id,
                    AuthorId = src.Track.AuthorId,
                    AlbumId = src.Track.AlbumId,
                    Title = src.Track.Title,
                    CreatedAt = src.Track.CreatedAt,
                    Duration = src.Track.Duration,
                    AvatarUrl = src.Track.AvatarUrl
                });

            CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TrackCount, opt => opt.MapFrom(src => src.TrackCount))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks.OrderByDescending(x => x.AddedDate)));//////////////////

            CreateMap<PlaylistDto, Playlist>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TrackCount, opt => opt.MapFrom(src => src.TrackCount))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));


            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.AlbumId))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));

            CreateMap<TrackDto, Track>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.AlbumId))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));


            CreateMap<Album, AlbumDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks))
                .ForMember(dest => dest.TrackCount, opt => opt.MapFrom(src => src.TrackCount));

            CreateMap<AlbumDto, Album>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks))
                .ForMember(dest => dest.TrackCount, opt => opt.MapFrom(src => src.TrackCount));

            CreateMap<Types.User, User>()
                .ForMember(d => d.Id, x => x.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(d => d.Email, x => x.MapFrom(src => src.Email))
                .ForMember(d => d.Name, x => x.MapFrom(src => src.Name))
                .ForMember(d => d.CreatedAt, x => x.MapFrom(src => src.CreatedAt.ToDateTime()))
                .ForMember(d => d.AvatarUrl, x => x.MapFrom(src => src.AvatarUrl))
                .ForMember(d => d.Bio, x => x.MapFrom(src => src.Bio));

            CreateMap<User, Types.User>()
                .ForMember(d => d.Id, x => x.MapFrom(src => src.Id.ToString()))
                .ForMember(d => d.Email, x => x.MapFrom(src => src.Email))
                .ForMember(d => d.Name, x => x.MapFrom(src => src.Name))
                .ForMember(d => d.CreatedAt, x => x.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt)))
                .ForMember(d => d.AvatarUrl, x => x.MapFrom(src => src.AvatarUrl))
                .ForMember(d => d.Bio, x => x.MapFrom(src => src.Bio));
        }
    }
}

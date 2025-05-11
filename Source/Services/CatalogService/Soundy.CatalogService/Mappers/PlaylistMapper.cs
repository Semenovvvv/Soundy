using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Service.Playlist;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.PlaylistDtos;
using Types;

namespace Soundy.CatalogService.Mappers
{
    public class PlaylistMapper : Profile
    {
        public PlaylistMapper()
        {
            CreateMap<CreateRequest, CreateRequestDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => Guid.Parse(s.AuthorId)));

            CreateMap<CreateResponseDto, CreateResponse>()
                .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist));



            CreateMap<CreateFavoriteRequest, CreateFavoriteRequestDto>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()));

            CreateMap<CreateFavoriteResponseDto, CreateFavoriteResponse>()
                .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist));



            CreateMap<GetByIdRequest, GetByIdRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)));

            CreateMap<GetByIdResponseDto, GetByIdResponse>()
                .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist));



            CreateMap<GetListByAuthorIdRequest, GetListByAuthorIdRequestDto>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => Guid.Parse(s.AuthorId)));

            CreateMap<GetListByAuthorIdResponseDto, GetListByAuthorIdResponse>()
                .ForMember(d => d.Playlists, o => o.MapFrom(s => s.Playlists));



            CreateMap<GetFavoriteRequest, GetFavoriteRequestDto>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => Guid.Parse(s.AuthorId)));

            CreateMap<GetFavoriteResponseDto, GetFavoriteResponse>()
                .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist));



            CreateMap<AddTrackRequest, AddTrackRequestDto>()
                .ForMember(d => d.PlaylistId, o => o.MapFrom(s => Guid.Parse(s.PlaylistId)))
                .ForMember(d => d.TrackId, o => o.MapFrom(s => Guid.Parse(s.TrackId)));

            CreateMap<AddTrackResponseDto, AddTrackResponse>()
                .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist));



            CreateMap<UpdateRequest, UpdateRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<UpdateResponseDto, UpdateResponse>()
                .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist));



            CreateMap<DeleteRequest, DeleteRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)));

            CreateMap<DeleteResponseDto, DeleteResponse>()
                .ForMember(d => d.IsSuccess, o => o.MapFrom(s => s.IsSuccess));



            CreateMap<Playlist, PlaylistDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => Guid.Parse(s.AuthorId)))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt.ToDateTime()))
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.AvatarUrl))
                .ForMember(d => d.TrackCount, o => o.MapFrom(s => s.TrackCount))
                .ForMember(d => d.Tracks, o => o.MapFrom(s => s.Tracks));

            CreateMap<PlaylistDto, Playlist>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => Timestamp.FromDateTime(s.CreatedAt.ToUniversalTime())))
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.AvatarUrl ?? string.Empty))
                .ForMember(d => d.TrackCount, o => o.MapFrom(s => s.TrackCount))
                .ForMember(d => d.Tracks, o => o.MapFrom(s => s.Tracks));



            CreateMap<Track, TrackDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => Guid.Parse(s.AuthorId)))
                .ForMember(d => d.AlbumId, o => o.MapFrom(s => Guid.Parse(s.AlbumId)))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt.ToDateTime()))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.AvatarUrl));

            CreateMap<TrackDto, Track>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.AlbumId, o => o.MapFrom(s => s.AlbumId.ToString()))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => Timestamp.FromDateTime(s.CreatedAt)))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.AvatarUrl ?? string.Empty));
        }
    }
}

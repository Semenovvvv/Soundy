using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Service.Playlist;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.PlaylistDtos;
using Soundy.CatalogService.Entities;
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

            CreateMap<GetLatestPlaylistsRequest, GetLatestPlaylistsRequestDto>()
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count));

            CreateMap<GetLatestPlaylistsResponseDto, GetLatestPlaylistsResponse>()
                .ForMember(dest => dest.Playlists, opt => opt.MapFrom(src => src.Playlists));

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

            CreateMap<SearchRequest, SearchRequestDto>()
                .ForMember(d => d.Pattern, o => o.MapFrom(s => s.Pattern))
                .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
                .ForMember(d => d.PageNum, o => o.MapFrom(s => s.PageNum));

            CreateMap<SearchResponseDto, SearchResponse>()
                .ForMember(d => d.Pattern, o => o.MapFrom(s => s.Pattern))
                .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
                .ForMember(d => d.PageNum, o => o.MapFrom(s => s.PageNum))
                .ForMember(d => d.Playlists, o => o.MapFrom(s => s.Playlists));

            CreateMap<Entities.Playlist, PlaylistDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TrackCount, opt => opt.MapFrom(src => src.TrackCount))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks.OrderByDescending(x => x.AddedDate)));

            CreateMap<PlaylistDto, Types.Playlist>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.User, o => o.MapFrom(s => s.Author))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt.ToTimestamp()))
                .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.AvatarUrl ?? string.Empty))
                .ForMember(d => d.TrackCount, o => o.MapFrom(s => s.TrackCount))
                .ForMember(d => d.Tracks, o => o.MapFrom(s => s.Tracks));
        }
    }
}

using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Service.Track;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.TrackDtos;
using Types;

namespace Soundy.CatalogService.Mappers
{
    public class TrackMapper : Profile
    {
        public TrackMapper()
        {
            CreateMap<CreateRequest, CreateRequestDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => Guid.Parse(src.AlbumId)))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

            CreateMap<CreateResponseDto, CreateResponse>()
                .ForMember(d => d.Track, o => o.MapFrom(s => s.Track));

            CreateMap<GetByIdRequest, GetByIdRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)));

            CreateMap<GetByIdResponseDto, GetByIdResponse>()
                .ForMember(d => d.Track, o => o.MapFrom(s => s.Track));

            CreateMap<UpdateRequest, UpdateRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title));

            CreateMap<UpdateResponseDto, UpdateResponse>()
                .ForMember(d => d.Track, o => o.MapFrom(s => s.Track));

            CreateMap<DeleteRequest, DeleteRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)));

            CreateMap<DeleteResponseDto, DeleteResponse>()
                .ForMember(d => d.Success, o => o.MapFrom(s => s.Success));

            CreateMap<SearchRequest, SearchRequestDto>()
                .ForMember(d => d.Pattern, o => o.MapFrom(s => s.Pattern))
                .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
                .ForMember(d => d.PageNum, o => o.MapFrom(s => s.PageNum));

            CreateMap<SearchResponseDto, SearchResponse>()
                .ForMember(d => d.Pattern, o => o.MapFrom(s => s.Pattern))
                .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
                .ForMember(d => d.PageNum, o => o.MapFrom(s => s.PageNum))
                .ForMember(d => d.Tracks, o => o.MapFrom(s => s.Tracks));

            //CreateMap<GetListByPlaylistRequest, GetListByPlaylistRequestDto>()
            //    .ForMember(d => d.PlaylistId, o => o.MapFrom(s => Guid.Parse(s.PlaylistId)));

            //CreateMap<GetListByPlaylistResponseDto, GetListByPlaylistResponse>()
            //    .ForMember(d => d.PlaylistId, o => o.MapFrom(s => s.PlaylistId.ToString()))
            //    .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist))
            //    .ForMember(d => d.Tracks, o => o.MapFrom(s => s.Tracks));

            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => Guid.Parse(src.User.Id)))
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => Guid.Parse(src.AlbumId)))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToDateTime()))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

            CreateMap<TrackDto, Track>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.AlbumId.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

            CreateMap<GetListByUserIdRequest, GetListByUserIdRequestDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)));

            CreateMap<GetListByUserIdResponseDto, GetListByUserIdResponse>()
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));
        }
    }
}

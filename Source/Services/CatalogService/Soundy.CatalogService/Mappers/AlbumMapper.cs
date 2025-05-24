using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Service.Album;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.AlbumDtos;
using Soundy.CatalogService.Entities;
using Types;

namespace Soundy.CatalogService.Mappers
{
    public class AlbumMapper : Profile
    {
        public AlbumMapper()
        {
            CreateMap<CreateRequest, CreateRequestDto>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => Guid.Parse(src.AuthorId)));

            CreateMap<CreateResponseDto, CreateResponse>()
                .ForMember(dest => dest.Album, opt => opt.MapFrom(src => src.Album));

            CreateMap<AddTrackRequest, AddTrackRequestDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.TrackId, opt => opt.MapFrom(src => Guid.Parse(src.TrackId)));

            CreateMap<AddTrackResponseDto, AddTrackResponse>()
                .ForMember(dest => dest.Album, opt => opt.MapFrom(src => src.Album));

            CreateMap<GetByIdRequest, GetByIdRequestDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<GetByIdResponseDto, GetByIdResponse>()
                .ForMember(dest => dest.Album, opt => opt.MapFrom(src => src.Album));

            CreateMap<GetByAuthorIdRequest, GetByAuthorIdRequestDto>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => Guid.Parse(src.AuthorId)));

            CreateMap<GetByAuthorIdResponseDto, GetByAuthorIdResponse>()
                .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => src.Albums));

            CreateMap<SearchRequest, SearchRequestDto>()
                .ForMember(dest => dest.Pattern, opt => opt.MapFrom(src => src.Pattern))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.PageNum, opt => opt.MapFrom(src => src.PageNum));

            CreateMap<SearchResponseDto, SearchResponse>()
                .ForMember(dest => dest.Pattern, opt => opt.MapFrom(src => src.Pattern))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.PageNum, opt => opt.MapFrom(src => src.PageNum))
                .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => src.Albums));

            CreateMap<GetLatestAlbumsRequest, GetLatestAlbumsRequestDto>()
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count));

            CreateMap<GetLatestAlbumsResponseDto, GetLatestAlbumsResponse>()
                .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => src.Albums));

            CreateMap<Entities.User, Types.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio));

            CreateMap<Entities.Track, Types.Track>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.AlbumId.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));

            CreateMap<Entities.Album, AlbumDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));

            CreateMap<AlbumDto, Types.Album>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));
        }
    }
}

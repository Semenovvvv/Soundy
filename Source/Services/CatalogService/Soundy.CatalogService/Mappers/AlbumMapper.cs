using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Service.Album;
using Soundy.CatalogService.Dto;
using Soundy.CatalogService.Dto.AlbumDtos;
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

            CreateMap<Album, AlbumDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToDateTime()))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.AvatarUrl));

            CreateMap<AlbumDto, Album>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));
        }
    }
}

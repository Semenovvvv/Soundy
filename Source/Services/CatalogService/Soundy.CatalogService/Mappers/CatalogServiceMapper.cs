using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Soundy.CatalogService.Dto.PlaylistDto;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.Mappers
{
    public class CatalogServiceMapper : Profile
    {
        public CatalogServiceMapper()
        {
            #region Playlist
            
            CreateMap<CreatePlaylistRequest, CreatePlaylistRequestDto>()
                .ForMember(dto => dto.AuthorId, x => x.MapFrom(src => Guid.Parse(src.AuthorId)))
                .ForMember(dto => dto.Name, x => x.MapFrom(src => src.Name));

            CreateMap<CreatePlaylistResponseDto, CreatePlaylistResponse>()
                .ForMember(x => x.Id, src => src.MapFrom(dto => dto.Id.ToString()))
                .ForMember(x => x.AuthorId, src => src.MapFrom(dto => dto.AuthorId.ToString()))
                .ForMember(x => x.Name, src => src.MapFrom(dto => dto.Name));

            #endregion

            #region Track

            CreateMap<Track, TrackResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => src.PlaylistId.ToString()))
                .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.UploadDate)));

            CreateMap<CreateTrackRequest, Track>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => Guid.Parse(src.PlaylistId)));

            #endregion
        }
    }
}

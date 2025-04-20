using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.Mappers
{
    public class TrackProfile : Profile
    {
        public TrackProfile()
        {
            CreateMap<Track, TrackResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => src.PlaylistId.ToString()))
                .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.UploadDate)));

            CreateMap<CreateTrackRequest, Track>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => Guid.Parse(src.PlaylistId)));
        }
    }
}

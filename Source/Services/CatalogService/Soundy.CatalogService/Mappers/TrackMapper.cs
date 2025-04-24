using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Soundy.CatalogService.Dto.TrackDto;
using Soundy.SharedLibrary.Contracts.Track;

namespace Soundy.CatalogService.Mappers
{
    public class TrackMapper : Profile
    {
        public TrackMapper()
        {
            CreateMap<CreateRequest, CreateRequestDto>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.UserId, o => o.MapFrom(s => Guid.Parse(s.UserId)))
                .ForMember(d => d.PlaylistId, o => o.MapFrom(s => Guid.Parse(s.PlaylistId)))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration));

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

            CreateMap<GetListByPlaylistRequest, GetListByPlaylistRequestDto>()
                .ForMember(d => d.PlaylistId, o => o.MapFrom(s => Guid.Parse(s.PlaylistId)));

            CreateMap<GetListByPlaylistResponseDto, GetListByPlaylistResponse>()
                .ForMember(d => d.PlaylistId, o => o.MapFrom(s => s.PlaylistId.ToString()))
                .ForMember(d => d.Playlist, o => o.MapFrom(s => s.Playlist))
                .ForMember(d => d.Tracks, o => o.MapFrom(s => s.Tracks));

            CreateMap<TrackData, TrackDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)))
                .ForMember(d => d.UserId, o => o.MapFrom(s => Guid.Parse(s.UserId)))
                .ForMember(d => d.PlaylistId, o => o.MapFrom(s => Guid.Parse(s.PlaylistId)))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt.ToDateTime()))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title));

            CreateMap<TrackDto, TrackData>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId.ToString()))
                .ForMember(d => d.PlaylistId, o => o.MapFrom(s => s.PlaylistId.ToString()))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => Timestamp.FromDateTime(s.CreatedAt)))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title));

            CreateMap<PlaylistDto, PlaylistData>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => Timestamp.FromDateTime(s.CreatedAt)))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
        }
    }
}

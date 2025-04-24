using AutoMapper;
using Soundy.CatalogService.Dto.PlaylistDto;
using Soundy.SharedLibrary.Contracts.Playlist;

namespace Soundy.CatalogService.Mappers
{
    public class PlaylistMapper : Profile
    {
        public PlaylistMapper()
        {
            // Create
            CreateMap<CreateRequest, CreateRequestDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => Guid.Parse(s.AuthorId)));

            CreateMap<CreateResponseDto, CreateResponse>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            // Get By Id
            CreateMap<GetByIdRequest, GetByIdRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)));

            CreateMap<GetByIdResponseDto, GetByIdResponse>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Tracks, o => o.MapFrom(s => s.Tracks));

            // Get List By Author Id
            CreateMap<GetListByAuthorIdRequest, GetListByAuthorIdRequestDto>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => Guid.Parse(s.AuthorId)));

            CreateMap<GetListByAuthorIdResponseDto, GetListByAuthorIdResponse>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.Playlists, o => o.MapFrom(s => s.Playlists));

            // Update
            CreateMap<UpdateRequest, UpdateRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<UpdateResponseDto, UpdateResponse>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            // Delete
            CreateMap<DeleteRequest, DeleteRequestDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Guid.Parse(s.Id)));

            CreateMap<DeleteResponseDto, DeleteResponse>()
                .ForMember(d => d.IsSuccess, o => o.MapFrom(s => s.IsSuccess));

            // PlaylistData mappings
            CreateMap<PlaylistDto, PlaylistData>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId.ToString()))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
        }
    }
}

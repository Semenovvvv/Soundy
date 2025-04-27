using AutoMapper;
using Soundy.SharedLibrary.Contracts.User;
using Soundy.UserService.Dto;

namespace Soundy.UserService.Mappers
{
    public class UserServiceMapper : Profile
    {
        public UserServiceMapper()
        {
            // CreateUser
            CreateMap<CreateRequest, CreateRequestDto>()
                .ForMember(dto => dto.UserName, x => x.MapFrom(src => src.Username))
                .ForMember(dto => dto.Email, x => x.MapFrom(src => src.Email));

            CreateMap<CreateResponseDto, CreateResponse>()
                .ForMember(src => src.Id, x => x.MapFrom(dto => dto.Id.ToString()))
                .ForMember(src => src.Username, x => x.MapFrom(dto => dto.UserName))
                .ForMember(src => src.Email, x => x.MapFrom(dto => dto.Email));

            // DeleteUser
            CreateMap<DeleteRequest, DeleteRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<DeleteResponseDto, DeleteResponse>()
                .ForMember(src => src.IsSuccess, x => x.MapFrom(dto => dto.IsSuccess));

            // GetUserById
            CreateMap<GetByIdRequest, GetByIdRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<GetByIdResponseDto, GetByIdResponse>()
                .ForMember(src => src.Id, x => x.MapFrom(dto => dto.Id.ToString()))
                .ForMember(src => src.Username, x => x.MapFrom(dto => dto.UserName))
                .ForMember(src => src.Email, x => x.MapFrom(dto => dto.Email));

            // SearchUsers
            CreateMap<SearchRequest, SearchRequestDto>()
                .ForMember(dto => dto.Pattern, x => x.MapFrom(src => src.Pattern))
                .ForMember(dto => dto.PageNumber, x => x.MapFrom(src => src.PageNumber))
                .ForMember(dto => dto.PageSize, x => x.MapFrom(src => src.PageSize));

            CreateMap<SearchResponseDto, SearchResponse>()
                .ForMember(src => src.Users, x => x.MapFrom(dto => dto.Users))
                .ForMember(src => src.PageNumber, x => x.MapFrom(dto => dto.PageNumber))
                .ForMember(src => src.PageSize, x => x.MapFrom(dto => dto.PageSize));

            // UpdateUser
            CreateMap<UpdateRequest, UpdateRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dto => dto.UserName, x => x.MapFrom(src => src.Username))
                .ForMember(dto => dto.Email, x => x.MapFrom(src => src.Email));

            CreateMap<UpdateResponseDto, UpdateResponse>()
                .ForMember(src => src.Id, x => x.MapFrom(dto => dto.Id.ToString()))
                .ForMember(src => src.Username, x => x.MapFrom(dto => dto.UserName))
                .ForMember(src => src.Email, x => x.MapFrom(dto => dto.Email));

            // UserData
            CreateMap<UserDto, UserData>()
                .ForMember(data => data.Id, x => x.MapFrom(dto => dto.Id.ToString()))
                .ForMember(data => data.Username, x => x.MapFrom(dto => dto.UserName))
                .ForMember(data => data.Email, x => x.MapFrom(dto => dto.Email));
        }
    }
}

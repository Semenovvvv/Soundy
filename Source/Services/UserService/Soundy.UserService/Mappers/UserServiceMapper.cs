using AutoMapper;
using Soundy.UserService.Dto;

namespace Soundy.UserService.Mappers
{
    public class UserServiceMapper : Profile
    {
        public UserServiceMapper()
        {
            // CreateUser
            CreateMap<CreateUserRequest, CreateUserRequestDto>()
                .ForMember(dto => dto.UserName, x => x.MapFrom(src => src.Username))
                .ForMember(dto => dto.Email, x => x.MapFrom(src => src.Email));

            CreateMap<CreateUserResponseDto, CreateUserResponse>()
                .ForMember(src => src.Id, x => x.MapFrom(dto => dto.Id.ToString()))
                .ForMember(src => src.Username, x => x.MapFrom(dto => dto.UserName))
                .ForMember(src => src.Email, x => x.MapFrom(dto => dto.Email));

            // DeleteUser
            CreateMap<DeleteUserRequest, DeleteUserRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<DeleteUserResponseDto, DeleteUserResponse>()
                .ForMember(src => src.IsSuccess, x => x.MapFrom(dto => dto.IsSuccess));

            // GetUserById
            CreateMap<GetUserByIdRequest, GetUserByIdRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<GetUserByIdResponseDto, GetUserByIdResponse>()
                .ForMember(src => src.Id, x => x.MapFrom(dto => dto.Id.ToString()))
                .ForMember(src => src.Username, x => x.MapFrom(dto => dto.UserName))
                .ForMember(src => src.Email, x => x.MapFrom(dto => dto.Email));

            // SearchUsers
            CreateMap<SearchUsersRequest, SearchUsersRequestDto>()
                .ForMember(dto => dto.Pattern, x => x.MapFrom(src => src.Pattern))
                .ForMember(dto => dto.PageNumber, x => x.MapFrom(src => src.PageNumber))
                .ForMember(dto => dto.PageSize, x => x.MapFrom(src => src.PageSize));

            CreateMap<SearchUsersResponseDto, SearchUsersResponse>()
                .ForMember(src => src.Users, x => x.MapFrom(dto => dto.Users))
                .ForMember(src => src.PageNumber, x => x.MapFrom(dto => dto.PageNumber))
                .ForMember(src => src.PageSize, x => x.MapFrom(dto => dto.PageSize));

            // UpdateUser
            CreateMap<UpdateUserRequest, UpdateUserRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dto => dto.UserName, x => x.MapFrom(src => src.Username))
                .ForMember(dto => dto.Email, x => x.MapFrom(src => src.Email));

            CreateMap<UpdateUserResponseDto, UpdateUserResponse>()
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

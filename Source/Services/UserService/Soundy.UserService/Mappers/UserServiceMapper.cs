using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Service.User;
using Soundy.UserService.Dto;
using Types;

namespace Soundy.UserService.Mappers
{
    public class UserServiceMapper : Profile
    {
        public UserServiceMapper()
        {
            // CreateUser
            CreateMap<CreateRequest, CreateRequestDto>()
                .ForMember(dto => dto.Name, x => x.MapFrom(src => src.Name))
                .ForMember(dto => dto.Bio, x => x.MapFrom(src => src.Bio))
                .ForMember(dto => dto.Email, x => x.MapFrom(src => src.Email))
                .ForMember(dto => dto.Id, x => x.MapFrom(src => src.Id));

            CreateMap<CreateResponseDto, CreateResponse>()
                .ForMember(src => src.User, x => x.MapFrom(dto => dto.User));

            // DeleteUser
            CreateMap<DeleteRequest, DeleteRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<DeleteResponseDto, DeleteResponse>()
                .ForMember(src => src.IsSuccess, x => x.MapFrom(dto => dto.IsSuccess));

            // GetUserById
            CreateMap<GetByIdRequest, GetByIdRequestDto>()
                .ForMember(dto => dto.Id, x => x.MapFrom(src => Guid.Parse(src.Id)));

            CreateMap<GetByIdResponseDto, GetByIdResponse>()
                .ForMember(src => src.User, x => x.MapFrom(dto => dto.User));

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
                .ForMember(dto => dto.Email, x => x.MapFrom(src => src.Email))
                .ForMember(dto => dto.Bio, x => x.MapFrom(src => src.Bio))
                .ForMember(dto => dto.AvatarUrl, x => x.MapFrom(src => src.AvatarUrl));

            CreateMap<UpdateResponseDto, UpdateResponse>()
                .ForMember(src => src.User, x => x.MapFrom(dto => dto.User));

            CreateMap<Types.User, UserDto>()
                .ForMember(d => d.Id, x => x.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(d => d.Email, x => x.MapFrom(src => src.Email))
                .ForMember(d => d.Name, x => x.MapFrom(src => src.Name))
                .ForMember(d => d.CreatedAt, x => x.MapFrom(src => src.CreatedAt.ToDateTime()))
                .ForMember(d => d.AvatarUrl, x => x.MapFrom(src => src.AvatarUrl))
                .ForMember(d => d.Bio, x => x.MapFrom(src => src.Bio));

            CreateMap<UserDto, Types.User>()
                .ForMember(d => d.Id, x => x.MapFrom(src => src.Id.ToString()))
                .ForMember(d => d.Email, x => x.MapFrom(src => src.Email))
                .ForMember(d => d.Name, x => x.MapFrom(src => src.Name))
                .ForMember(d => d.CreatedAt, x => x.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt)))
                .ForMember(d => d.AvatarUrl, x => x.MapFrom(src => src.AvatarUrl))
                .ForMember(d => d.Bio, x => x.MapFrom(src => src.Bio));

            CreateMap<Entities.User, UserDto>()
                .ForMember(d => d.Id, x => x.MapFrom(src => src.Id))
                .ForMember(d => d.Email, x => x.MapFrom(src => src.Email))
                .ForMember(d => d.Name, x => x.MapFrom(src => src.Name))
                .ForMember(d => d.CreatedAt, x => x.MapFrom(src => src.CreatedAt))
                .ForMember(d => d.AvatarUrl, x => x.MapFrom(src => src.AvatarUrl))
                .ForMember(d => d.Bio, x => x.MapFrom(src => src.Bio));

            // GetLatestUsers
            CreateMap<GetLatestUsersRequest, GetLatestUsersRequestDto>()
                .ForMember(dto => dto.Count, x => x.MapFrom(src => src.Count));

            CreateMap<GetLatestUsersResponseDto, GetLatestUsersResponse>()
                .ForMember(src => src.Users, x => x.MapFrom(dto => dto.Users));
        }
    }
}

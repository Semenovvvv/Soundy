using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Iam;
using Service.Playlist;
using Soundy.UserService.DataAccess;
using Soundy.UserService.Dto;
using Soundy.UserService.Entities;
using Soundy.UserService.Interfaces;

namespace Soundy.UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;
        private readonly PlaylistGrpcService.PlaylistGrpcServiceClient _playlistService;
        private readonly IAMGrpcService.IAMGrpcServiceClient _iamService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IDbContextFactory<DatabaseContext> dbFactory, 
            PlaylistGrpcService.PlaylistGrpcServiceClient playlistService, 
            IAMGrpcService.IAMGrpcServiceClient iamService,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _playlistService = playlistService;
            _iamService = iamService;
            _dbFactory = dbFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateResponseDto> CreateUserAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                var user = new User()
                {
                    Id = !string.IsNullOrEmpty(dto.Id) ? Guid.Parse(dto.Id) : Guid.NewGuid(),
                    Email = dto.Email,
                    Name = dto.Name,
                    Bio = dto.Bio,
                    AvatarUrl = string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                if (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email, ct) is not null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument,
                        $"User already exists. Email = {dto.Email}"));

                await dbContext.Users.AddAsync(user, ct);
                await _playlistService.CreateFavoriteAsync(new CreateFavoriteRequest() { AuthorId = user.Id.ToString() });
                await dbContext.SaveChangesAsync(ct);

                return new CreateResponseDto { User = _mapper.Map<UserDto>(user) };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "RPC error creating user: {Message}", rpcEx.Status.Detail);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async Task<GetByIdResponseDto> GetUserById(GetByIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == dto.Id, cancellationToken: ct);

            if (user is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

            return new GetByIdResponseDto { User = _mapper.Map<UserDto>(user) };
        }

        public async Task<UpdateResponseDto> UpdateUser(UpdateRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken: ct);
                if (user is null)
                    throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

                if (string.IsNullOrEmpty(dto.UserName))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Username is required"));

                user.Name = dto.UserName;
                
                if (dto.Bio != null)
                {
                    user.Bio = dto.Bio;
                }
                
                if (dto.AvatarUrl != null)
                {
                    user.AvatarUrl = dto.AvatarUrl;
                }

                await dbContext.SaveChangesAsync(ct);

                try
                {
                    var iamUpdateRequest = new UpdateUserDataRequest
                    {
                        UserId = user.Id.ToString(),
                        Username = user.Name
                    };

                    _logger.LogInformation("Updating user data in IAM for user ID: {UserId}", user.Id);
                    var iamResponse = await _iamService.UpdateUserDataAsync(iamUpdateRequest, cancellationToken: ct);
                    
                    if (!iamResponse.Success)
                    {
                        _logger.LogWarning("Failed to update user data in IAM: {ErrorMessage}", iamResponse.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating user data in IAM for user ID: {UserId}", user.Id);
                }

                return new UpdateResponseDto { User = _mapper.Map<UserDto>(user) };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "RPC error updating user with ID: {UserId}", dto.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", dto.Id);
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async Task<DeleteResponseDto> DeleteUser(DeleteRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken: ct);

                if (user == null)
                    throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

                var deleteResult = await dbContext.Users.Where(x => x.Id == dto.Id).ExecuteDeleteAsync(ct);

                if (deleteResult > 0)
                {
                    try
                    {
                        _logger.LogInformation("Deleting user from IAM service: {UserId}", dto.Id);
                        var deleteUserRequest = new Service.Iam.DeleteUserRequest { UserId = dto.Id.ToString() };
                        var iamResponse = await _iamService.DeleteUserAsync(deleteUserRequest, cancellationToken: ct);
                        
                        if (!iamResponse.Success)
                        {
                            _logger.LogWarning("Failed to delete user from IAM: {ErrorMessage}", iamResponse.ErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting user from IAM service: {UserId}", dto.Id);
                    }
                }

                return new DeleteResponseDto()
                {
                    IsSuccess = deleteResult > 0
                };
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "RPC error deleting user with ID: {UserId}", dto.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", dto.Id);
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async Task<SearchResponseDto> SearchUsers(SearchRequestDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Pattern))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Pattern is empty"));

            if (dto.PageSize < 1 || dto.PageNumber < 1)
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid pagination parameters"));

            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var query = dbContext.Users
                .AsNoTracking()
                .Where(user => EF.Functions.Like(user.Name, $"%{dto.Pattern}%"));

            var users = await query
                .OrderBy(u => u.Name)
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => _mapper.Map<UserDto>(x))
                .ToListAsync(ct);

            return new SearchResponseDto
            {
                Users = users,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }

        /// <summary>
        /// Получает список последних зарегистрированных пользователей
        /// </summary>
        /// <param name="dto">Параметры запроса с количеством записей</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список последних зарегистрированных пользователей</returns>
        public async Task<GetLatestUsersResponseDto> GetLatestUsersAsync(GetLatestUsersRequestDto dto, CancellationToken ct = default)
        {
            int count = dto.Count > 0 ? dto.Count : 10; // По умолчанию 10, если передано некорректное значение
            
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
            
            var latestUsers = await dbContext.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync(ct);

            var userDtos = latestUsers.Select(user => _mapper.Map<UserDto>(user)).ToList();

            return new GetLatestUsersResponseDto
            {
                Users = userDtos
            };
        }
    }
}

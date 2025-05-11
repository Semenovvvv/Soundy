using Grpc.Core;
using Microsoft.EntityFrameworkCore;
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

        public UserService(IDbContextFactory<DatabaseContext> dbFactory, PlaylistGrpcService.PlaylistGrpcServiceClient playlistService)
        {
            _playlistService = playlistService;
            _dbFactory = dbFactory;
        }

        // TODO Вынести создание плейлиста в api gateway
        public async Task<CreateResponseDto> CreateUserAsync(CreateRequestDto dto, CancellationToken ct = default)
        {
            try
            {
                var user = new User()
                {
                    Email = dto.Email,
                    Name = dto.UserName,
                    CreatedAt = DateTime.UtcNow
                };

                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                if (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email, ct) is not null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument,
                        $"User already exists. Email = {dto.Email}"));

                await dbContext.Users.AddAsync(user, ct);
                await _playlistService.CreateFavoriteAsync(new CreateFavoriteRequest() { AuthorId = user.Id.ToString() });
                await dbContext.SaveChangesAsync(ct);

                return new CreateResponseDto()
                {
                    Id = user.Id,
                    UserName = user.Name,
                    Email = user.Email
                };
            }
            catch (RpcException rpcEx)
            {
                throw;
            }
            catch (Exception ex)
            {
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

            return new GetByIdResponseDto()
            {
                Id = user.Id,
                UserName = user.Name,
                Email = user.Email
            };
        }

        public async Task<UpdateResponseDto> UpdateUser(UpdateRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken: ct);
            if (user is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.UserName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Arguments null"));

            user.Name = dto.UserName;
            user.Email = dto.Email;

            await dbContext.SaveChangesAsync(ct);

            return new UpdateResponseDto()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Name
            };
        }

        public async Task<DeleteResponseDto> DeleteUser(DeleteRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken: ct);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

            var a = await dbContext.Users.Where(x => x.Id == dto.Id).ExecuteDeleteAsync(ct);

            await dbContext.SaveChangesAsync(ct);

            return new DeleteResponseDto()
            {
                IsSuccess = true
            };
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
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.Name
                })
                .ToListAsync(ct);

            return new SearchResponseDto()
            {
                Users = users,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }
    }
}

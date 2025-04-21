using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Soundy.UserService.DataAccess;
using Soundy.UserService.Dto;
using Soundy.UserService.Entities;
using Soundy.UserService.Interfaces;

namespace Soundy.UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;

        public UserService(IDbContextFactory<DatabaseContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<CreateUserResponseDto> CreateUserAsync(CreateUserRequestDto dto, CancellationToken ct = default)
        {
            var user = new User()
            {
                Email = dto.Email,
                UserName = dto.UserName,
                CreatedAt = DateTime.UtcNow
            };

            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            if (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email, ct) is not null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"User already exists. Email = {dto.Email}"));

            await dbContext.Users.AddAsync(user, ct);
            await dbContext.SaveChangesAsync(ct);

            return new CreateUserResponseDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<GetUserByIdResponseDto> GetUserById(GetUserByIdRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == dto.Id, cancellationToken: ct);

            if (user is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

            return new GetUserByIdResponseDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<UpdateUserResponseDto> UpdateUser(UpdateUserRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken: ct);
            if (user is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.UserName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Arguments null"));

            user.UserName = dto.UserName;
            user.Email = dto.Email;

            await dbContext.SaveChangesAsync(ct);

            return new UpdateUserResponseDto()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }

        public async Task<DeleteUserResponseDto> DeleteUser(DeleteUserRequestDto dto, CancellationToken ct = default)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken: ct);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User not found. Id = {dto.Id}"));

            var a = await dbContext.Users.Where(x => x.Id == dto.Id).ExecuteDeleteAsync(ct);

            await dbContext.SaveChangesAsync(ct);

            return new DeleteUserResponseDto()
            {
                IsSuccess = true
            };
        }

        public async Task<SearchUsersResponseDto> SearchUsers(SearchUsersRequestDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Pattern))
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Pattern is empty"));

            if (dto.PageSize < 1 || dto.PageNumber < 1)
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid pagination parameters"));

            await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

            var query = dbContext.Users
                .AsNoTracking()
                .Where(user => EF.Functions.Like(user.UserName, $"%{dto.Pattern}%"));

            var users = await query
                .OrderBy(u => u.UserName)
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x => new UserDto()
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName
                })
                .ToListAsync(ct);

            return new SearchUsersResponseDto()
            {
                Users = users,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }
    }
}

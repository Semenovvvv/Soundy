using Microsoft.EntityFrameworkCore;
using Soundy.SharedLibrary.Common.Response;
using Soundy.SharedLibrary.Enums;
using Soundy.UserService.DataAccess;
using Soundy.UserService.Dto;
using Soundy.UserService.Entities;

namespace Soundy.UserService.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IDbContextFactory<DatabaseContext> _dbFactory;

        public UserService(
            IConfiguration configuration,
            ILogger<UserService> logger,
            IDbContextFactory<DatabaseContext> dbFactory)
        {
            _logger = logger;
            _dbFactory = dbFactory;
        }

        public async Task<Response<Entities.User>> GetUserById(Guid userId, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                var user = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: ct);

                return user is null
                    ? Response<User>.Fail(ResponseStatus.NotFound, $"User with ID '{userId}' not found.")
                    : Response<User>.Success(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error fetching user by ID: {userId}");
                return Response<Entities.User>.Fail(ResponseStatus.InternalError, "An internal server error occurred.");
            }
        }

        public async Task<Response<User>> GetUserByLogin(string login, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return Response<User>.Fail(ResponseStatus.InvalidInput, "Login cannot be empty.");
            }

            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                var user = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Login == login, cancellationToken: ct);

                return user == null
                    ? Response<User>.Fail(ResponseStatus.NotFound, $"User '{login}' not found.")
                    : Response<User>.Success(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error fetching user by login: {login}");
                return Response<User>.Fail(ResponseStatus.InternalError, "Internal server error.");
            }
        }

        public async Task<Response> UpdateUserProfile(Guid userId, UpdateUserDto dto, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: ct);
                if (user == null)
                    return Response.Fail(ResponseStatus.NotFound, "User not found");

                if (dto.Email == null)
                    return Response.Fail(ResponseStatus.InvalidInput, "Invalid email format");

                user.Login = dto.Username ?? user.Login;
                user.Email = dto.Email ?? user.Email;

                await dbContext.SaveChangesAsync(ct);

                return Response.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user profile {userId}");
                return Response.Fail(ResponseStatus.InternalError, "Internal server error");
            }
        }

        public async Task<Response> DeleteUser(Guid userId, CancellationToken ct = default)
        {
            try
            {
                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: ct);

                if (user == null)
                    return Response.Fail(ResponseStatus.NotFound, "User not found");

                //var result = await dbContext.Users.ExecuteDeleteAsync(user, );  DeleteAsync(user);

                var a = await dbContext.Users.Where(x => x.Id == userId).ExecuteDeleteAsync(ct);

                await dbContext.SaveChangesAsync(ct);

                return Response.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}");
                return Response.Fail(ResponseStatus.InternalError, "Internal server error");
            }
        }

        public async Task<PaginatedResponse<User>> GetUsersByName(
            string userNamePattern,
            int pageNumber,
            int pageSize, 
            CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userNamePattern))
                    return PaginatedResponse<User>.Fail(ResponseStatus.InvalidInput, "Search pattern cannot be empty");

                if (pageNumber < 1 || pageSize < 1)
                    return PaginatedResponse<User>.Fail(ResponseStatus.InvalidInput, "Invalid pagination parameters");

                await using var dbContext = await _dbFactory.CreateDbContextAsync(ct);

                var query = dbContext.Users
                    .AsNoTracking()
                    .Where(user => EF.Functions.Like(user.Login, $"%{userNamePattern}%"));

                var totalRecords = await query.CountAsync(ct);

                var users = await query
                    .OrderBy(u => u.Login)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

                return PaginatedResponse<User>.Success(users, totalRecords, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching users by name '{userNamePattern}'");
                return PaginatedResponse<User>.Fail(ResponseStatus.InternalError, "Internal server error");
            }
        }
    }
}

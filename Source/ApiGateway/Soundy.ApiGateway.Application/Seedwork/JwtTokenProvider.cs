using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Soundy.ApiGateway.Application.Seedwork.Abstractions;
using Soundy.ApiGateway.Domain.Entities;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Soundy.SharedLibrary.Jwt;

namespace Soundy.ApiGateway.Application.Seedwork
{
    internal sealed class JwtTokenProvider : ITokenProvider
    {
        private readonly JwtOptions _options;

        public JwtTokenProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        string ITokenProvider.GenerateToken(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.UtcNow.Add(_options.Expires));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}

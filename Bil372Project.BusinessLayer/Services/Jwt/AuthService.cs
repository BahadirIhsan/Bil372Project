using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Dtos.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Bil372Project.BusinessLayer.Services.Jwt;

public class AuthService : IAuthService
{
    private readonly IAppUserService _userService;
    private readonly IUserSessionService _sessionService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IAppUserService userService,
        IUserSessionService sessionService,
        IOptions<JwtSettings> jwtOptions)
    {
        _userService = userService;
        _sessionService = sessionService;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthTokenDto?> CreateTokenAsync(LoginDto loginDto)
    {
        var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
        if (user is null)
            return null;

        var session = _sessionService.CreateSession(user);
        var claims = _sessionService.BuildClaims(session);

        return GenerateToken(claims);
    }

    private AuthTokenDto GenerateToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(tokenDescriptor);

        return new AuthTokenDto
        {
            AccessToken = accessToken,
            ExpiresAt = expires
        };
    }
}
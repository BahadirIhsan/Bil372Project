using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Dtos.Jwt;

namespace Bil372Project.BusinessLayer.Services.Jwt;

public interface IAuthService
{
    Task<AuthTokenDto?> CreateTokenAsync(LoginDto loginDto);
}
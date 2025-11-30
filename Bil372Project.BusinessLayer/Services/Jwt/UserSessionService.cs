using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Dtos.Jwt;
using Bil372Project.EntityLayer.Entities;

namespace Bil372Project.BusinessLayer.Services.Jwt;

public class UserSessionService : IUserSessionService
{
    public UserSessionDto CreateSession(AppUser user)
    {
        return new UserSessionDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public IEnumerable<Claim> BuildClaims(UserSessionDto session)
    {
        return new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, session.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, session.Email),
            new(JwtRegisteredClaimNames.UniqueName, session.FullName)
        };
    }
}
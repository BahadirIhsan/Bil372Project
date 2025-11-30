using System.Security.Claims;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Dtos.Jwt;
using Bil372Project.EntityLayer.Entities;

namespace Bil372Project.BusinessLayer.Services.Jwt;
public interface IUserSessionService
{
    UserSessionDto CreateSession(AppUser user);
    IEnumerable<Claim> BuildClaims(UserSessionDto session);
}
namespace Bil372Project.BusinessLayer.Dtos.Jwt;

public class AuthTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
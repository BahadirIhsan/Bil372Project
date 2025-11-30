namespace Bil372Project.BusinessLayer.Dtos.Jwt;

// csharp
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
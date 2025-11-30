namespace Bil372Project.BusinessLayer.Dtos.Jwt;

public class UserSessionDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

}
namespace Bil372Project.BusinessLayer.Dtos;

public class AdminUserOptions
{
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
namespace Bil372Project.BusinessLayer.Dtos;

public class AdminUserListItemDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Bio { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime UpdatedAt { get; set; }
}
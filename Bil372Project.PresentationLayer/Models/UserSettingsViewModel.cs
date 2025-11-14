namespace Bil372Project.PresentationLayer.Models;

public class UserSettingsViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public DateTime? BirthDate { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Bio { get; set; }
}
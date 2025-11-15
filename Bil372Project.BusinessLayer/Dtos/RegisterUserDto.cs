namespace Bil372Project.BusinessLayer.Dtos;

public class RegisterUserDto : UserSettingsDto
{
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
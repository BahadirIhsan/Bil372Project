using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.EntityLayer.Entities;

namespace Bil372Project.BusinessLayer.Services;

public interface IAppUserService
{
    // login için tuttuğum servis
    Task<AppUser?> LoginAsync(string email, string password);
    // kayıt için tuttuğum servis
    Task<(bool success, string? errorMessage)> ChangePasswordAsync(ChangePasswordDto dto);    
    Task<UserSettingsDto?> GetSettingsAsync(int userId);

    Task<(bool success, string? errorMessage)> UpdateSettingsAsync(int userId, UserSettingsDto input);

    Task<(bool success, string? errorMessage)> RegisterAsync(RegisterUserDto dto);
}
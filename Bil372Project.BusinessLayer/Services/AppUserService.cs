using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class AppUserService : IAppUserService
{
    private readonly AppDbContext _context;
    private readonly IValidator<RegisterUserDto> _registerValidator;
    private readonly IValidator<UserSettingsDto> _settingsValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;

    public AppUserService(
        AppDbContext context,
        IValidator<RegisterUserDto> registerValidator,
        IValidator<UserSettingsDto> settingsValidator,
        IValidator<ChangePasswordDto> changePasswordValidator)
    {
        _context = context;
        _registerValidator = registerValidator;
        _settingsValidator = settingsValidator;
        _changePasswordValidator = changePasswordValidator;
    }
    
    public async Task<AppUser?> LoginAsync(string email, string password)
    {
        // 1) Önce kullanıcıyı email'den bul
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return null;

        // 2) Şifre kontrolünü her yerde kullandığın VerifyPassword ile yap
        if (!VerifyPassword(user.Password, password))
            return null;

        return user;
    }


    public async Task<ServiceResult> RegisterAsync(RegisterUserDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ServiceResult.Failed(validation.Errors);

        var user = new AppUser
        {
            FullName    = dto.FullName,
            Email       = dto.Email,
            Password    = HashPassword(dto.Password), // DİKKAT: artık hash kullan
            PhoneNumber = dto.PhoneNumber,
            BirthDate   = dto.BirthDate,
            City        = dto.City,
            Country     = dto.Country,
            Bio         = dto.Bio
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<UserSettingsDto?> GetSettingsAsync(int userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        return new UserSettingsDto
        {
            FullName    = user.FullName,
            Email       = user.Email,
            PhoneNumber = user.PhoneNumber,
            BirthDate   = user.BirthDate,
            City        = user.City,
            Country     = user.Country,
            Bio         = user.Bio
        };
    }

    public async Task<ServiceResult> UpdateSettingsAsync(
        int userId,
        UserSettingsDto input)
    {
        var validation = await _settingsValidator.ValidateAsync(input);
        if (!validation.IsValid)
            return ServiceResult.Failed(validation.Errors);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return ServiceResult.Failed(string.Empty, "Kullanıcı bulunamadı.");

        // Email başka kullanıcıda var mı?
        bool emailTaken = await _context.Users
            .AnyAsync(u => u.Email == input.Email && u.Id != userId);

        if (emailTaken)
            return ServiceResult.Failed(nameof(UserSettingsDto.Email), "Bu e-posta başka bir kullanıcı tarafından kullanılıyor.");

        user.FullName    = input.FullName;
        user.Email       = input.Email;
        user.PhoneNumber = input.PhoneNumber;
        user.BirthDate   = input.BirthDate;
        user.City        = input.City;
        user.Country     = input.Country;
        user.Bio         = input.Bio;

        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var validation = await _changePasswordValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ServiceResult.Failed(validation.Errors);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
        if (user == null)
            return ServiceResult.Failed(string.Empty, "Kullanıcı bulunamadı.");

        if (!VerifyPassword(user.Password, dto.CurrentPassword))
            return ServiceResult.Failed(nameof(dto.CurrentPassword), "Mevcut şifreniz yanlış.");

        user.Password = HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    private bool VerifyPassword(string storedPassword, string inputPassword)
    {
        // İLERİDE: hash karşılaştırma
        return storedPassword == inputPassword;
    }

    private string HashPassword(string password)
    {
        // İLERİDE: hash üretme (BCrypt vs.)
        return password;
    }
}
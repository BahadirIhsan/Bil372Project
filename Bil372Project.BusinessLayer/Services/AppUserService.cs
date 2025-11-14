using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class AppUserService : IAppUserService
{
    private readonly AppDbContext _context;
    
    public AppUserService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<AppUser?> LoginAsync(string email, string password)
    {
        // ileride password hash'e çevrildiğinde burada kontrol edeceğiz
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }

    public async Task<(bool success, string? errorMessage)> RegisterAsync(
        string fullName,
        string email,
        string password)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            return (false, "Bu e-posta ile kayıtlı kullanıcı var.");
        }

        var user = new AppUser
        {
            FullName = fullName,
            Email = email,
            Password = password // ileride hash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, null);
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

        public async Task<(bool success, string? errorMessage)> UpdateSettingsAsync(
            int userId,
            UserSettingsDto input)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (false, "Kullanıcı bulunamadı.");

            // Email başka kullanıcıda var mı?
            bool emailTaken = await _context.Users
                .AnyAsync(u => u.Email == input.Email && u.Id != userId);

            if (emailTaken)
                return (false, "Bu e-posta başka bir kullanıcı tarafından kullanılıyor.");

            // UserName başka kullanıcıda var mı?
            

            
            user.FullName    = input.FullName;
            user.Email       = input.Email;
            user.PhoneNumber = input.PhoneNumber;
            user.BirthDate   = input.BirthDate;
            user.City        = input.City;
            user.Country     = input.Country;
            user.Bio         = input.Bio;

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool success, string? errorMessage)> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return (false, "Kullanıcı bulunamadı.");

            if (!VerifyPassword(user.Password, currentPassword))
                return (false, "Mevcut şifreniz yanlış.");

            user.Password = HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return (true, null);
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
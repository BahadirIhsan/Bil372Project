using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class AdminService : IAdminService
{
    private const int MaxPageSize = 50;
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<AdminUserListItemDto>> SearchUsersAsync(string? email, string? fullName, string? city, string? phone, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = ClampPageSize(pageSize);

        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            query = query.Where(u => u.Email.ToLower().Contains(normalizedEmail));
        }
        
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var normalizedFullName = fullName.Trim().ToLowerInvariant();
            query = query.Where(u => u.FullName.ToLower().Contains(normalizedFullName));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalizedCity = city.Trim().ToLowerInvariant();
            query = query.Where(u => u.City != null && u.City.ToLower().Contains(normalizedCity));
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(normalizedPhone))
            {
                normalizedPhone = phone.Trim();
            }

            query = query.Where(u => u.PhoneNumber != null
                                     && u.PhoneNumber.Replace(" ", string.Empty)
                                         .Replace("-", string.Empty)
                                         .Replace("(", string.Empty)
                                         .Replace(")", string.Empty)
                                         .Replace("+", string.Empty)
                                         .Contains(normalizedPhone));
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserListItemDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                BirthDate = u.BirthDate,
                City = u.City,
                Country = u.Country,
                Bio = u.Bio,
                UpdatedAt = u.UpdatedAt,
                IsAdmin = u.IsAdmin
            })
            .ToListAsync();

        return PaginatedResult<AdminUserListItemDto>.Create(users, totalCount, page, pageSize);
    }

    public async Task<PaginatedResult<AdminDietPlanDto>> SearchDietPlansAsync(
        string? userEmail,
        string? breakfastKeyword,
        string? lunchKeyword,
        string? dinnerKeyword,
        string? snackKeyword,
        string sortOrder,
        int page,
        int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = ClampPageSize(pageSize);

        var query = _context.UserDietPlans
            .AsNoTracking()
            .Include(p => p.UserMeasure)
            .ThenInclude(m => m.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userEmail))
        {
            var normalizedEmail = userEmail.Trim().ToLowerInvariant();
            query = query.Where(p => p.UserMeasure.User.Email.ToLower().Contains(normalizedEmail));
        }

        if (!string.IsNullOrWhiteSpace(breakfastKeyword))
        {
            var normalizedKeyword = breakfastKeyword.Trim().ToLowerInvariant();
            query = query.Where(p => p.Breakfast.ToLower().Contains(normalizedKeyword));
        }
        
        if (!string.IsNullOrWhiteSpace(lunchKeyword))
        {
            var normalizedKeyword = lunchKeyword.Trim().ToLowerInvariant();
            query = query.Where(p => p.Lunch.ToLower().Contains(normalizedKeyword));
        }

        if (!string.IsNullOrWhiteSpace(dinnerKeyword))
        {
            var normalizedKeyword = dinnerKeyword.Trim().ToLowerInvariant();
            query = query.Where(p => p.Dinner.ToLower().Contains(normalizedKeyword));
        }

        if (!string.IsNullOrWhiteSpace(snackKeyword))
        {
            var normalizedKeyword = snackKeyword.Trim().ToLowerInvariant();
            query = query.Where(p => p.Snack.ToLower().Contains(normalizedKeyword));
        }

        var totalCount = await query.CountAsync();

        query = sortOrder?.Trim().ToLowerInvariant() == "asc"
            ? query.OrderBy(p => p.GeneratedAt)
            : query.OrderByDescending(p => p.GeneratedAt);
        
        var plans = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new AdminDietPlanDto
            {
                Id = p.Id,
                UserEmail = p.UserMeasure.User.Email,
                UserFullName = p.UserMeasure.User.FullName,
                Breakfast = p.Breakfast,
                Lunch = p.Lunch,
                Dinner = p.Dinner,
                Snack = p.Snack,
                ModelVersion = p.ModelVersion,
                GeneratedAt = p.GeneratedAt
            })
            .ToListAsync();

        return PaginatedResult<AdminDietPlanDto>.Create(plans, totalCount, page, pageSize);
    }

    private static int ClampPageSize(int pageSize)
    {
        if (pageSize <= 0)
            return MaxPageSize;

        return Math.Min(pageSize, MaxPageSize);
    }
}
using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.BusinessLayer.Services;

public interface IAdminService
{
    Task<PaginatedResult<AdminUserListItemDto>> SearchUsersAsync(string? email, int page, int pageSize);
    Task<PaginatedResult<AdminDietPlanDto>> SearchDietPlansAsync(string? userEmail, string? breakfastKeyword, int page, int pageSize);
}
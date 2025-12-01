using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer;

namespace Bil372Project.BusinessLayer.Services;

public interface IAdminService
{
    Task<PaginatedResult<AdminUserListItemDto>> SearchUsersAsync(
        string? email,
        string? fullName,
        string? city,
        string? phone,
        int page,
        int pageSize);

    Task<ServiceResult> GrantAdminAsync(int userId);
    Task<PaginatedResult<AdminDietPlanDto>> SearchDietPlansAsync(
        string? userEmail,
        string? breakfastKeyword,
        string? lunchKeyword,
        string? dinnerKeyword,
        string? snackKeyword,
        string sortOrder,
        int page,
        int pageSize);
    
}
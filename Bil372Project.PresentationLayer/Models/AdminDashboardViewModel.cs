using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.PresentationLayer.Models;

public class AdminDashboardViewModel
{
    public string? UserEmailFilter { get; set; }
    public string? UserFullNameFilter { get; set; }
    public string? UserCityFilter { get; set; }
    public string? UserPhoneFilter { get; set; }
    public bool UserSearchApplied { get; set; }
    public int UserPage { get; set; } = 1;
    public PaginatedResult<AdminUserListItemDto> Users { get; set; } = PaginatedResult<AdminUserListItemDto>.Create(new List<AdminUserListItemDto>(), 0, 1, 50);

    public string? DietUserEmail { get; set; }
    public string? BreakfastKeyword { get; set; }
    public string? LunchKeyword { get; set; }
    public string? DinnerKeyword { get; set; }
    public string? SnackKeyword { get; set; }
    public int DietPage { get; set; } = 1;
    public PaginatedResult<AdminDietPlanDto> DietPlans { get; set; } = PaginatedResult<AdminDietPlanDto>.Create(new List<AdminDietPlanDto>(), 0, 1, 50);
    public string DietSortOrder { get; set; } = "desc";
    public bool HasDietSearch { get; set; }
}
using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.PresentationLayer.Models;

public class AdminDashboardViewModel
{
    public string? UserEmailFilter { get; set; }
    public int UserPage { get; set; } = 1;
    public PaginatedResult<AdminUserListItemDto> Users { get; set; } = PaginatedResult<AdminUserListItemDto>.Create(new List<AdminUserListItemDto>(), 0, 1, 50);

    public string? DietUserEmail { get; set; }
    public string? BreakfastKeyword { get; set; }
    public int DietPage { get; set; } = 1;
    public PaginatedResult<AdminDietPlanDto> DietPlans { get; set; } = PaginatedResult<AdminDietPlanDto>.Create(new List<AdminDietPlanDto>(), 0, 1, 50);
}
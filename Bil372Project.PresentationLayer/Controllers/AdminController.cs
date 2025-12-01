using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Services;
using Bil372Project.PresentationLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Bil372Project.PresentationLayer.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private const int PageSize = 50;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? userEmail,
        string? userFullName = null,
        string? userCity = null,
        string? userPhone = null,
        bool userSearch = false,
        int userPage = 1,
        string? dietUserEmail = null,
        string? breakfastKeyword = null,
        string? lunchKeyword = null,
        string? dinnerKeyword = null,
        string? snackKeyword = null,
        string sortOrder = "desc",
        bool dietSearch = false,
        int dietPage = 1)
    {
        var hasUserSearch = userSearch
                            || !string.IsNullOrWhiteSpace(userEmail)
                            || !string.IsNullOrWhiteSpace(userFullName)
                            || !string.IsNullOrWhiteSpace(userCity)
                            || !string.IsNullOrWhiteSpace(userPhone);

        var users = hasUserSearch
            ? await _adminService.SearchUsersAsync(userEmail, userFullName, userCity, userPhone, userPage, PageSize)
            : PaginatedResult<AdminUserListItemDto>.Create(Array.Empty<AdminUserListItemDto>(), 0, 1, PageSize);
        
        var dietPlans = dietSearch
            ? await _adminService.SearchDietPlansAsync(
                dietUserEmail,
                breakfastKeyword,
                lunchKeyword,
                dinnerKeyword,
                snackKeyword,
                sortOrder,
                dietPage,
                PageSize)
            : PaginatedResult<AdminDietPlanDto>.Create(new List<AdminDietPlanDto>(), 0, dietPage, PageSize);
        
        var model = new AdminDashboardViewModel
        {
            UserEmailFilter = userEmail,
            UserFullNameFilter = userFullName,
            UserCityFilter = userCity,
            UserPhoneFilter = userPhone,
            UserSearchApplied = hasUserSearch,
            UserPage = userPage,
            Users = users,
            DietUserEmail = dietUserEmail,
            BreakfastKeyword = breakfastKeyword,
            LunchKeyword = lunchKeyword,
            DinnerKeyword = dinnerKeyword,
            SnackKeyword = snackKeyword,
            DietPage = dietPage,
            DietPlans = dietPlans,
            DietSortOrder = sortOrder,
            HasDietSearch = dietSearch
        };

        return View(model);
    }
}
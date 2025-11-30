using Bil372Project.BusinessLayer.Services;
using Bil372Project.PresentationLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        int userPage = 1,
        string? dietUserEmail = null,
        string? breakfastKeyword = null,
        int dietPage = 1)
    {
        var users = await _adminService.SearchUsersAsync(userEmail, userPage, PageSize);
        var dietPlans = await _adminService.SearchDietPlansAsync(dietUserEmail, breakfastKeyword, dietPage, PageSize);

        var model = new AdminDashboardViewModel
        {
            UserEmailFilter = userEmail,
            UserPage = userPage,
            Users = users,
            DietUserEmail = dietUserEmail,
            BreakfastKeyword = breakfastKeyword,
            DietPage = dietPage,
            DietPlans = dietPlans
        };

        return View(model);
    }
}
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bil372Project.PresentationLayer.Models;

namespace Bil372Project.PresentationLayer.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Values()
    {
        return View();
    }

    public IActionResult History()
    {
        return View();
    }

    public IActionResult NewProgram()
    {
        return View();
    }

    public IActionResult Settings()
    {
        return View();
    }
}
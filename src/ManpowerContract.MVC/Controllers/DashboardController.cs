using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.MVC.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.FullName = HttpContext.Session.GetString("FullName") ?? "User";
        ViewBag.RoleName = HttpContext.Session.GetString("RoleName") ?? "";
        return View();
    }
}

using ManpowerContract.MVC.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.MVC.Controllers;

[SkipAuthFilter]
public class ErrorController : Controller
{
    [Route("Error/{statusCode?}")]
    public IActionResult Index(int? statusCode)
    {
        ViewBag.StatusCode = statusCode ?? 500;
        ViewBag.Message = statusCode switch
        {
            404 => "The page you are looking for was not found.",
            403 => "You do not have permission to access this resource.",
            401 => "You are not authorized. Please login.",
            _ => "An unexpected error occurred."
        };
        return View("Error");
    }
}

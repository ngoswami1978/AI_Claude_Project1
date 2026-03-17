using ManpowerContract.Application.DTOs.Auth;
using ManpowerContract.MVC.Filters;
using ManpowerContract.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.MVC.Controllers;

[SkipAuthFilter]
public class AccountController : Controller
{
    private readonly ApiClientService _api;

    public AccountController(ApiClientService api) => _api = api;

    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Index", "Dashboard");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var result = await _api.PostAsync<LoginResponseDto>("api/auth/login", dto);
            if (!result.Success || result.Data == null)
            {
                ViewBag.Error = result.Message ?? "Invalid credentials.";
                return View(dto);
            }

            var loginResponse = result.Data;

            // Store JWT token in session
            HttpContext.Session.SetString("JwtToken", loginResponse.Token);
            HttpContext.Session.SetString("UserId", loginResponse.UserId.ToString());
            HttpContext.Session.SetString("FullName", loginResponse.FullName);
            HttpContext.Session.SetString("Email", loginResponse.Email);
            HttpContext.Session.SetString("RoleName", loginResponse.RoleName);

            // Store permissions as comma-separated for HasPermission filter
            if (loginResponse.Permissions != null && loginResponse.Permissions.Any())
            {
                HttpContext.Session.SetString("Permissions", string.Join(",", loginResponse.Permissions));
            }

            return RedirectToAction("Index", "Dashboard");
        }
        catch (HttpRequestException)
        {
            ViewBag.Error = "Unable to connect to the API server. Please ensure the API is running.";
            return View(dto);
        }
        catch (TaskCanceledException)
        {
            ViewBag.Error = "The API server did not respond in time. Please try again.";
            return View(dto);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"An unexpected error occurred: {ex.Message}";
            return View(dto);
        }
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View("~/Views/Shared/AccessDenied.cshtml");
    }
}

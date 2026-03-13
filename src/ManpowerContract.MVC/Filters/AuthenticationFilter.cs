using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManpowerContract.MVC.Filters;

/// <summary>
/// Checks if user has a valid JWT token in session. Redirects to login if not.
/// Apply via [ServiceFilter(typeof(AuthenticationFilter))] or globally.
/// </summary>
public class AuthenticationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Skip filter for controllers/actions marked with [SkipAuthFilter]
        var hasSkip = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is SkipAuthFilterAttribute);
        if (hasSkip) return;

        var token = context.HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SkipAuthFilterAttribute : Attribute { }

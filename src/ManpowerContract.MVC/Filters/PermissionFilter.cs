using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManpowerContract.MVC.Filters;

/// <summary>
/// Checks if user has a specific permission. Usage: [HasPermission("USER_MGMT.Create")]
/// Permission strings are stored in session as comma-separated values.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _permission;

    public HasPermissionAttribute(string permission) => _permission = permission;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permissionsJson = context.HttpContext.Session.GetString("Permissions");
        if (string.IsNullOrEmpty(permissionsJson))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var permissions = permissionsJson.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (!permissions.Contains(_permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/AccessDenied.cshtml",
                StatusCode = 403
            };
        }
    }
}

using ManpowerContract.Application.DTOs.Common;
using ManpowerContract.Application.DTOs.Permission;
using ManpowerContract.MVC.Filters;
using ManpowerContract.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.MVC.Controllers;

public class RolePermissionController : Controller
{
    private readonly ApiClientService _api;

    public RolePermissionController(ApiClientService api) => _api = api;

    [HasPermission("PERM_MGMT.View")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Roles = await _api.GetListAsync<DropdownDto>("api/lookup/roles");
        ViewBag.Users = await _api.GetListAsync<DropdownDto>("api/lookup/users");
        return View();
    }

    [HttpGet]
    [HasPermission("PERM_MGMT.View")]
    public async Task<IActionResult> GetPermissions(int roleId)
    {
        var result = await _api.GetAsync<List<RolePermissionDto>>($"api/rolepermission/{roleId}");
        return Json(result);
    }

    [HttpPost]
    [HasPermission("PERM_MGMT.Update")]
    public async Task<IActionResult> Save([FromBody] RolePermissionSaveDto dto)
    {
        var result = await _api.PostAsync<bool>("api/rolepermission/save", dto);
        return Json(result);
    }

    [HttpPost]
    [HasPermission("PERM_MGMT.Update")]
    public async Task<IActionResult> CopyFromRole([FromBody] CopyPermissionDto dto)
    {
        var result = await _api.PostAsync<bool>("api/rolepermission/copy-from-role", dto);
        return Json(result);
    }

    [HttpPost]
    [HasPermission("PERM_MGMT.Update")]
    public async Task<IActionResult> CopyFromUser([FromBody] CopyPermissionDto dto)
    {
        var result = await _api.PostAsync<bool>("api/rolepermission/copy-from-user", dto);
        return Json(result);
    }
}

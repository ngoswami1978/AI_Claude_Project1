using ManpowerContract.Application.DTOs.Role;
using ManpowerContract.MVC.Filters;
using ManpowerContract.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.MVC.Controllers;

public class RoleManagementController : Controller
{
    private readonly ApiClientService _api;

    public RoleManagementController(ApiClientService api) => _api = api;

    [HasPermission("ROLE_MGMT.View")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [HasPermission("ROLE_MGMT.View")]
    public async Task<IActionResult> Search([FromBody] RoleSearchDto dto)
    {
        var result = await _api.PostAsync<List<RoleResponseDto>>("api/role/search", dto);
        return Json(result);
    }

    [HttpGet]
    [HasPermission("ROLE_MGMT.View")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _api.GetAsync<RoleResponseDto>($"api/role/{id}");
        return Json(result);
    }

    [HttpPost]
    [HasPermission("ROLE_MGMT.Create")]
    public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
    {
        var result = await _api.PostAsync<int>("api/role", dto);
        return Json(result);
    }

    [HttpPost]
    [HasPermission("ROLE_MGMT.Update")]
    public async Task<IActionResult> Update([FromBody] RoleUpdateDto dto)
    {
        var result = await _api.PutAsync<bool>("api/role", dto);
        return Json(result);
    }

    [HttpPost]
    [HasPermission("ROLE_MGMT.Disable")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _api.DeleteAsync<bool>($"api/role/{id}");
        return Json(result);
    }
}

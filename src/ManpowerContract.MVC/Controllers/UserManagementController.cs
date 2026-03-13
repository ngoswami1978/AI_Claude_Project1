using ManpowerContract.Application.DTOs.Common;
using ManpowerContract.Application.DTOs.User;
using ManpowerContract.MVC.Filters;
using ManpowerContract.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.MVC.Controllers;

public class UserManagementController : Controller
{
    private readonly ApiClientService _api;

    public UserManagementController(ApiClientService api) => _api = api;

    [HasPermission("USER_MGMT.View")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Roles = await _api.GetListAsync<DropdownDto>("api/lookup/roles");
        ViewBag.Departments = await _api.GetListAsync<DropdownDto>("api/lookup/departments");
        return View();
    }

    [HttpPost]
    [HasPermission("USER_MGMT.View")]
    public async Task<IActionResult> Search([FromBody] UserSearchDto dto)
    {
        var result = await _api.PostAsync<List<UserResponseDto>>("api/user/search", dto);
        return Json(result);
    }

    [HttpGet]
    [HasPermission("USER_MGMT.View")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _api.GetAsync<UserResponseDto>($"api/user/{id}");
        return Json(result);
    }

    [HttpPost]
    [HasPermission("USER_MGMT.Create")]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        var result = await _api.PostAsync<int>("api/user", dto);
        return Json(result);
    }

    [HttpPost]
    [HasPermission("USER_MGMT.Update")]
    public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
    {
        var result = await _api.PutAsync<bool>("api/user", dto);
        return Json(result);
    }

    [HttpPost]
    [HasPermission("USER_MGMT.Disable")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _api.DeleteAsync<bool>($"api/user/{id}");
        return Json(result);
    }
}

using ManpowerContract.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LookupController : ControllerBase
{
    private readonly IRoleService _roleService;

    public LookupController(IRoleService roleService) => _roleService = roleService;

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var data = await _roleService.GetRoleDropdownAsync();
        return Ok(data);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var data = await _roleService.GetUserDropdownAsync();
        return Ok(data);
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        var data = await _roleService.GetDepartmentDropdownAsync();
        return Ok(data);
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetModules()
    {
        var data = await _roleService.GetModulesAsync();
        return Ok(data);
    }
}

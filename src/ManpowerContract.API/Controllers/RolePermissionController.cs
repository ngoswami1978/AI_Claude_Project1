using ManpowerContract.Application.DTOs.Permission;
using ManpowerContract.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManpowerContract.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolePermissionController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolePermissionController(IRoleService roleService) => _roleService = roleService;

    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetPermissions(int roleId)
    {
        var result = await _roleService.GetPermissionsAsync(roleId);
        return Ok(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SavePermissions([FromBody] RolePermissionSaveDto dto)
    {
        var result = await _roleService.SavePermissionsAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("copy-from-role")]
    public async Task<IActionResult> CopyFromRole([FromBody] CopyPermissionDto dto)
    {
        var result = await _roleService.CopyPermissionsFromRoleAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("copy-from-user")]
    public async Task<IActionResult> CopyFromUser([FromBody] CopyPermissionDto dto)
    {
        var result = await _roleService.CopyPermissionsFromUserAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
    }
}

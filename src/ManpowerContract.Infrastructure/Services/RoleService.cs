using System.Text.Json;
using ManpowerContract.Application.Common;
using ManpowerContract.Application.DTOs.Common;
using ManpowerContract.Application.DTOs.Permission;
using ManpowerContract.Application.DTOs.Role;
using ManpowerContract.Application.Interfaces.Repositories;
using ManpowerContract.Application.Interfaces.Services;
using ManpowerContract.Application.Models;
using Microsoft.Extensions.Logging;

namespace ManpowerContract.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repo;
    private readonly ILogger<RoleService> _logger;

    public RoleService(IRoleRepository repo, ILogger<RoleService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<RoleResponseDto>>> SearchAsync(RoleSearchDto filters)
    {
        var roles = await _repo.SearchAsync(filters);
        return ApiResponse<IEnumerable<RoleResponseDto>>.Ok(
            roles.Select(r => new RoleResponseDto
            {
                RoleId = r.RoleId, RoleName = r.RoleName,
                Description = r.Description, IsActive = r.IsActive, CreatedDate = r.CreatedDate
            }));
    }

    public async Task<ApiResponse<RoleResponseDto>> GetByIdAsync(int id)
    {
        var r = await _repo.GetByIdAsync(id);
        return r != null
            ? ApiResponse<RoleResponseDto>.Ok(new RoleResponseDto
            {
                RoleId = r.RoleId, RoleName = r.RoleName,
                Description = r.Description, IsActive = r.IsActive, CreatedDate = r.CreatedDate
            })
            : ApiResponse<RoleResponseDto>.Fail("Role not found.");
    }

    public async Task<ApiResponse<bool>> CreateAsync(RoleCreateDto dto, int? userId)
    {
        var model = new RoleModel { RoleName = dto.RoleName, Description = dto.Description, IsActive = dto.IsActive };
        var (result, error) = await _repo.InsertAsync(model);
        return result == 1 ? ApiResponse<bool>.Ok(true, "Role created.") : ApiResponse<bool>.Fail(error);
    }

    public async Task<ApiResponse<bool>> UpdateAsync(RoleUpdateDto dto, int? userId)
    {
        var model = new RoleModel { RoleId = dto.RoleId, RoleName = dto.RoleName, Description = dto.Description, IsActive = dto.IsActive };
        var (result, error) = await _repo.UpdateAsync(model);
        return result == 1 ? ApiResponse<bool>.Ok(true, "Role updated.") : ApiResponse<bool>.Fail(error);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, int? userId)
    {
        var (result, error) = await _repo.DeleteAsync(id, userId);
        return result == 1 ? ApiResponse<bool>.Ok(true, "Role deleted.") : ApiResponse<bool>.Fail(error);
    }

    public async Task<ApiResponse<bool>> SavePermissionsAsync(RolePermissionSaveDto dto, int? userId)
    {
        var json = JsonSerializer.Serialize(dto.Permissions, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var (result, error) = await _repo.SavePermissionsAsync(dto.RoleId, json, userId);
        return result == 1 ? ApiResponse<bool>.Ok(true, "Permissions saved.") : ApiResponse<bool>.Fail(error);
    }

    public async Task<ApiResponse<IEnumerable<RolePermissionDto>>> GetPermissionsAsync(int roleId)
    {
        var perms = await _repo.GetPermissionsByRoleAsync(roleId);
        return ApiResponse<IEnumerable<RolePermissionDto>>.Ok(
            perms.Select(p => new RolePermissionDto
            {
                RolePermissionId = p.RolePermissionId, ModuleId = p.ModuleId,
                ModuleCode = p.ModuleCode, ModuleName = p.ModuleName,
                ParentModuleId = p.ParentModuleId, ParentModuleName = p.ParentModuleName,
                CanCreate = p.CanCreate, CanDisable = p.CanDisable,
                CanView = p.CanView, CanUpdate = p.CanUpdate, CanDownload = p.CanDownload
            }));
    }

    public async Task<ApiResponse<bool>> CopyPermissionsFromRoleAsync(CopyPermissionDto dto, int? userId)
    {
        if (dto.SourceRoleId == null) return ApiResponse<bool>.Fail("Source role is required.");
        if (dto.SourceRoleId == dto.TargetRoleId) return ApiResponse<bool>.Fail("Source and target roles cannot be the same.");
        var (result, error) = await _repo.CopyPermissionsFromRoleAsync(dto.SourceRoleId.Value, dto.TargetRoleId, userId);
        return result == 1 ? ApiResponse<bool>.Ok(true, "Permissions copied from role.") : ApiResponse<bool>.Fail(error);
    }

    public async Task<ApiResponse<bool>> CopyPermissionsFromUserAsync(CopyPermissionDto dto, int? userId)
    {
        if (dto.SourceUserId == null) return ApiResponse<bool>.Fail("Source user is required.");
        var (result, error) = await _repo.CopyPermissionsFromUserAsync(dto.SourceUserId.Value, dto.TargetRoleId, userId);
        return result == 1 ? ApiResponse<bool>.Ok(true, "Permissions copied from user.") : ApiResponse<bool>.Fail(error);
    }

    public async Task<IEnumerable<DropdownDto>> GetRoleDropdownAsync() => await _repo.GetRoleDropdownAsync();
    public async Task<IEnumerable<DropdownDto>> GetUserDropdownAsync() => await _repo.GetUserDropdownAsync();
    public async Task<IEnumerable<DropdownDto>> GetDepartmentDropdownAsync() => await _repo.GetDepartmentDropdownAsync();
    public async Task<IEnumerable<ModuleModel>> GetModulesAsync() => await _repo.GetModulesAsync();
}

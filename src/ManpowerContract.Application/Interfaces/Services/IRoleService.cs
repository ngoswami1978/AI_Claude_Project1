using ManpowerContract.Application.Common;
using ManpowerContract.Application.DTOs.Common;
using ManpowerContract.Application.DTOs.Permission;
using ManpowerContract.Application.DTOs.Role;
using ManpowerContract.Application.Models;

namespace ManpowerContract.Application.Interfaces.Services;

public interface IRoleService
{
    Task<ApiResponse<IEnumerable<RoleResponseDto>>> SearchAsync(RoleSearchDto filters);
    Task<ApiResponse<RoleResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> CreateAsync(RoleCreateDto dto, int? userId);
    Task<ApiResponse<bool>> UpdateAsync(RoleUpdateDto dto, int? userId);
    Task<ApiResponse<bool>> DeleteAsync(int id, int? userId);
    Task<ApiResponse<bool>> SavePermissionsAsync(RolePermissionSaveDto dto, int? userId);
    Task<ApiResponse<IEnumerable<RolePermissionDto>>> GetPermissionsAsync(int roleId);
    Task<ApiResponse<bool>> CopyPermissionsFromRoleAsync(CopyPermissionDto dto, int? userId);
    Task<ApiResponse<bool>> CopyPermissionsFromUserAsync(CopyPermissionDto dto, int? userId);
    Task<IEnumerable<DropdownDto>> GetRoleDropdownAsync();
    Task<IEnumerable<DropdownDto>> GetUserDropdownAsync();
    Task<IEnumerable<DropdownDto>> GetDepartmentDropdownAsync();
    Task<IEnumerable<ModuleModel>> GetModulesAsync();
}

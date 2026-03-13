using ManpowerContract.Application.DTOs.Common;
using ManpowerContract.Application.DTOs.Role;
using ManpowerContract.Application.Models;

namespace ManpowerContract.Application.Interfaces.Repositories;

public interface IRoleRepository : IRepository<RoleModel>
{
    Task<IEnumerable<RoleModel>> SearchAsync(RoleSearchDto filters);
    Task<(int Result, string Error)> SavePermissionsAsync(int roleId, string permissionsJson, int? userId);
    Task<IEnumerable<RolePermissionModel>> GetPermissionsByRoleAsync(int roleId);
    Task<(int Result, string Error)> CopyPermissionsFromRoleAsync(int sourceRoleId, int targetRoleId, int? userId);
    Task<(int Result, string Error)> CopyPermissionsFromUserAsync(int sourceUserId, int targetRoleId, int? userId);
    Task<IEnumerable<DropdownDto>> GetRoleDropdownAsync();
    Task<IEnumerable<DropdownDto>> GetUserDropdownAsync();
    Task<IEnumerable<DropdownDto>> GetDepartmentDropdownAsync();
    Task<IEnumerable<ModuleModel>> GetModulesAsync();
}

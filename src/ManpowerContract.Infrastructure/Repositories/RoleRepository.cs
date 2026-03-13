using System.Data;
using ManpowerContract.Application.DTOs.Common;
using ManpowerContract.Application.DTOs.Role;
using ManpowerContract.Application.Interfaces.Repositories;
using ManpowerContract.Application.Models;
using ManpowerContract.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ManpowerContract.Infrastructure.Repositories;

public class RoleRepository : BaseRepository<RoleModel>, IRoleRepository
{
    public RoleRepository(IConfiguration config) : base(config) { }

    protected override RoleModel MapReader(SqlDataReader r) => new()
    {
        RoleId = r.GetSafeInt32("ROLE_ID"),
        RoleName = r.GetSafeString("ROLE_NAME"),
        Description = r.GetSafeString("DESCRIPTION"),
        IsActive = r.GetSafeBool("IS_ACTIVE"),
        CreatedDate = r.GetSafeDateTime("C_DATETIME")
    };

    public async Task<IEnumerable<RoleModel>> SearchAsync(RoleSearchDto f)
        => await QueryAsync("usp_Role_SEARCH", cmd =>
        {
            cmd.Parameters.AddWithValue("@RoleName", (object?)f.RoleName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", (object?)f.IsActive ?? DBNull.Value);
        });

    public async Task<RoleModel?> GetByIdAsync(int id)
        => await QuerySingleAsync("usp_Role_GETBYID", cmd => cmd.Parameters.AddWithValue("@RoleId", id));

    public async Task<IEnumerable<RoleModel>> GetAllAsync()
        => await QueryAsync("usp_Role_SEARCH");

    public async Task<(int Result, string Error)> InsertAsync(RoleModel entity)
        => await ExecuteSpAsync("usp_Role_INSERT", cmd =>
        {
            cmd.Parameters.AddWithValue("@RoleName", entity.RoleName);
            cmd.Parameters.AddWithValue("@Description", (object?)entity.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
            cmd.Parameters.AddWithValue("@CUserId", DBNull.Value);
        });

    public async Task<(int Result, string Error)> UpdateAsync(RoleModel entity)
        => await ExecuteSpAsync("usp_Role_UPDATE", cmd =>
        {
            cmd.Parameters.AddWithValue("@RoleId", entity.RoleId);
            cmd.Parameters.AddWithValue("@RoleName", entity.RoleName);
            cmd.Parameters.AddWithValue("@Description", (object?)entity.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
            cmd.Parameters.AddWithValue("@MUserId", DBNull.Value);
        });

    public async Task<(int Result, string Error)> DeleteAsync(int id, int? userId)
        => await ExecuteSpAsync("usp_Role_DELETE", cmd =>
        {
            cmd.Parameters.AddWithValue("@RoleId", id);
            cmd.Parameters.AddWithValue("@MUserId", (object?)userId ?? DBNull.Value);
        });

    public async Task<(int Result, string Error)> SavePermissionsAsync(int roleId, string permissionsJson, int? userId)
        => await ExecuteSpAsync("usp_RolePermission_SAVE", cmd =>
        {
            cmd.Parameters.AddWithValue("@RoleId", roleId);
            cmd.Parameters.AddWithValue("@PermJson", permissionsJson);
            cmd.Parameters.AddWithValue("@CUserId", (object?)userId ?? DBNull.Value);
        });

    public async Task<IEnumerable<RolePermissionModel>> GetPermissionsByRoleAsync(int roleId)
    {
        var list = new List<RolePermissionModel>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_RolePermission_GETBYROLE", con) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@RoleId", roleId);
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new RolePermissionModel
            {
                RolePermissionId = r.GetSafeInt32("ROLE_PERMISSION_ID"),
                ModuleId = r.GetSafeInt32("MODULE_ID"),
                ModuleCode = r.GetSafeString("MODULE_CODE"),
                ModuleName = r.GetSafeString("MODULE_NAME"),
                ParentModuleId = r.GetSafeNullableInt32("PARENT_MODULE_ID"),
                ParentModuleName = r.GetSafeString("PARENT_MODULE_NAME"),
                CanCreate = r.GetSafeBool("CAN_CREATE"),
                CanDisable = r.GetSafeBool("CAN_DISABLE"),
                CanView = r.GetSafeBool("CAN_VIEW"),
                CanUpdate = r.GetSafeBool("CAN_UPDATE"),
                CanDownload = r.GetSafeBool("CAN_DOWNLOAD")
            });
        }
        return list;
    }

    public async Task<(int Result, string Error)> CopyPermissionsFromRoleAsync(int sourceRoleId, int targetRoleId, int? userId)
        => await ExecuteSpAsync("usp_RolePermission_COPYFROMROLE", cmd =>
        {
            cmd.Parameters.AddWithValue("@SourceRoleId", sourceRoleId);
            cmd.Parameters.AddWithValue("@TargetRoleId", targetRoleId);
            cmd.Parameters.AddWithValue("@CUserId", (object?)userId ?? DBNull.Value);
        });

    public async Task<(int Result, string Error)> CopyPermissionsFromUserAsync(int sourceUserId, int targetRoleId, int? userId)
        => await ExecuteSpAsync("usp_RolePermission_COPYFROMUSER", cmd =>
        {
            cmd.Parameters.AddWithValue("@SourceUserId", sourceUserId);
            cmd.Parameters.AddWithValue("@TargetRoleId", targetRoleId);
            cmd.Parameters.AddWithValue("@CUserId", (object?)userId ?? DBNull.Value);
        });

    public async Task<IEnumerable<DropdownDto>> GetRoleDropdownAsync()
    {
        var list = new List<DropdownDto>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_Lookup_Role", con) { CommandType = CommandType.StoredProcedure };
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            list.Add(new DropdownDto { Value = r.GetSafeInt32("Value"), Text = r.GetSafeString("Text") });
        return list;
    }

    public async Task<IEnumerable<DropdownDto>> GetUserDropdownAsync()
    {
        var list = new List<DropdownDto>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_Lookup_User", con) { CommandType = CommandType.StoredProcedure };
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            list.Add(new DropdownDto { Value = r.GetSafeInt32("Value"), Text = r.GetSafeString("Text") });
        return list;
    }

    public async Task<IEnumerable<DropdownDto>> GetDepartmentDropdownAsync()
    {
        var list = new List<DropdownDto>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_Lookup_Department", con) { CommandType = CommandType.StoredProcedure };
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            list.Add(new DropdownDto { Value = r.GetSafeInt32("Value"), Text = r.GetSafeString("Text") });
        return list;
    }

    public async Task<IEnumerable<ModuleModel>> GetModulesAsync()
    {
        var list = new List<ModuleModel>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_Lookup_Module", con) { CommandType = CommandType.StoredProcedure };
        await con.OpenAsync();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            list.Add(new ModuleModel
            {
                ModuleId = r.GetSafeInt32("MODULE_ID"),
                ModuleCode = r.GetSafeString("MODULE_CODE"),
                ModuleName = r.GetSafeString("MODULE_NAME"),
                ParentModuleId = r.GetSafeNullableInt32("PARENT_MODULE_ID"),
                IconClass = r.GetSafeString("ICON_CLASS"),
                UrlPath = r.GetSafeString("URL_PATH"),
                DisplayOrder = r.GetSafeInt32("DISPLAY_ORDER")
            });
        return list;
    }
}

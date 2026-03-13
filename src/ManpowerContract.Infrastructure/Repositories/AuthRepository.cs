using System.Data;
using ManpowerContract.Application.Interfaces.Repositories;
using ManpowerContract.Application.Models;
using ManpowerContract.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ManpowerContract.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly string _conn;

    public AuthRepository(IConfiguration config)
        => _conn = config.GetConnectionString("DefaultConnection")!;

    public async Task<LoginResultModel?> ValidateLoginAsync(string email)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_Auth_LOGIN", con) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Email", email);
        var result = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var error = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(result); cmd.Parameters.Add(error);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new LoginResultModel
            {
                UserId = reader.GetSafeInt32("USER_ID"),
                FullName = reader.GetSafeString("FULL_NAME"),
                Email = reader.GetSafeString("EMAIL"),
                PasswordHash = reader.GetSafeString("PASSWORD_HASH"),
                PasswordSalt = reader.GetSafeString("PASSWORD_SALT"),
                RoleId = reader.GetSafeInt32("ROLE_ID"),
                RoleName = reader.GetSafeString("ROLE_NAME"),
                IsActive = reader.GetSafeBool("IS_ACTIVE")
            };
        }
        return null;
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId)
    {
        var permissions = new List<string>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_Auth_GetUserPermissions", con) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var code = reader.GetSafeString("MODULE_CODE");
            if (reader.GetSafeBool("CAN_CREATE"))   permissions.Add($"{code}.Create");
            if (reader.GetSafeBool("CAN_DISABLE"))   permissions.Add($"{code}.Disable");
            if (reader.GetSafeBool("CAN_VIEW"))      permissions.Add($"{code}.View");
            if (reader.GetSafeBool("CAN_UPDATE"))     permissions.Add($"{code}.Update");
            if (reader.GetSafeBool("CAN_DOWNLOAD"))   permissions.Add($"{code}.Download");
        }
        return permissions;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string newHash, string newSalt)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("usp_Auth_CHANGE_PASSWORD", con) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@NewHash", newHash);
        cmd.Parameters.AddWithValue("@NewSalt", newSalt);
        var result = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var error = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(result); cmd.Parameters.Add(error);
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return (int)result.Value == 1;
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("UPDATE MST_USER SET LAST_LOGIN_DT = GETDATE() WHERE USER_ID = @UserId", con);
        cmd.Parameters.AddWithValue("@UserId", userId);
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}

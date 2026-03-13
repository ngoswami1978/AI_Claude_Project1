using System.Data;
using ManpowerContract.Application.DTOs.User;
using ManpowerContract.Application.Interfaces.Repositories;
using ManpowerContract.Application.Models;
using ManpowerContract.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ManpowerContract.Infrastructure.Repositories;

public class UserRepository : BaseRepository<UserModel>, IUserRepository
{
    public UserRepository(IConfiguration config) : base(config) { }

    protected override UserModel MapReader(SqlDataReader r) => new()
    {
        UserId = r.GetSafeInt32("USER_ID"),
        FullName = r.GetSafeString("FULL_NAME"),
        Email = r.GetSafeString("EMAIL"),
        RoleId = r.GetSafeInt32("ROLE_ID"),
        RoleName = r.GetSafeString("ROLE_NAME"),
        DepartmentId = r.GetSafeNullableInt32("DEPARTMENT_ID"),
        DepartmentName = r.GetSafeString("DEPARTMENT_NAME"),
        IsActive = r.GetSafeBool("IS_ACTIVE"),
        CreatedDate = r.GetSafeDateTime("C_DATETIME")
    };

    public async Task<IEnumerable<UserModel>> SearchAsync(UserSearchDto f)
        => await QueryAsync("usp_User_SEARCH", cmd =>
        {
            cmd.Parameters.AddWithValue("@FullName", (object?)f.FullName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)f.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoleId", (object?)f.RoleId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartmentId", (object?)f.DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", (object?)f.IsActive ?? DBNull.Value);
        });

    public async Task<UserModel?> GetByIdAsync(int id)
        => await QuerySingleAsync("usp_User_GETBYID", cmd => cmd.Parameters.AddWithValue("@UserId", id));

    public async Task<IEnumerable<UserModel>> GetAllAsync()
        => await QueryAsync("usp_User_SEARCH");

    public async Task<(int Result, string Error)> InsertAsync(UserModel entity)
        => await ExecuteSpAsync("usp_User_INSERT", cmd =>
        {
            cmd.Parameters.AddWithValue("@FullName", entity.FullName);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
            cmd.Parameters.AddWithValue("@PasswordSalt", entity.PasswordSalt);
            cmd.Parameters.AddWithValue("@RoleId", entity.RoleId);
            cmd.Parameters.AddWithValue("@DepartmentId", (object?)entity.DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
            cmd.Parameters.AddWithValue("@CUserId", DBNull.Value);
        });

    public async Task<(int Result, string Error)> UpdateAsync(UserModel entity)
        => await ExecuteSpAsync("usp_User_UPDATE", cmd =>
        {
            cmd.Parameters.AddWithValue("@UserId", entity.UserId);
            cmd.Parameters.AddWithValue("@FullName", entity.FullName);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@RoleId", entity.RoleId);
            cmd.Parameters.AddWithValue("@DepartmentId", (object?)entity.DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
            cmd.Parameters.AddWithValue("@MUserId", DBNull.Value);
        });

    public async Task<(int Result, string Error)> DeleteAsync(int id, int? userId)
        => await ExecuteSpAsync("usp_User_DELETE", cmd =>
        {
            cmd.Parameters.AddWithValue("@UserId", id);
            cmd.Parameters.AddWithValue("@MUserId", (object?)userId ?? DBNull.Value);
        });

    public async Task<bool> EmailExistsAsync(string email, int excludeUserId = 0)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM MST_USER WITH (NOLOCK) WHERE EMAIL = @Email AND USER_ID != @ExcludeId AND IS_DELETED = 0", con);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@ExcludeId", excludeUserId);
        await con.OpenAsync();
        return (int)(await cmd.ExecuteScalarAsync())! > 0;
    }
}

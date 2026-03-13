using ManpowerContract.Application.Common;
using ManpowerContract.Application.DTOs.User;
using ManpowerContract.Application.Interfaces.Repositories;
using ManpowerContract.Application.Interfaces.Services;
using ManpowerContract.Application.Models;
using ManpowerContract.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;

namespace ManpowerContract.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repo, ILogger<UserService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<UserResponseDto>>> SearchAsync(UserSearchDto filters)
    {
        var users = await _repo.SearchAsync(filters);
        var dtos = users.Select(u => MapToDto(u));
        return ApiResponse<IEnumerable<UserResponseDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<UserResponseDto>> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user != null
            ? ApiResponse<UserResponseDto>.Ok(MapToDto(user))
            : ApiResponse<UserResponseDto>.Fail("User not found.");
    }

    public async Task<ApiResponse<bool>> CreateAsync(UserCreateDto dto, int? userId)
    {
        if (await _repo.EmailExistsAsync(dto.Email))
            return ApiResponse<bool>.Fail("Email already exists.");

        var hash = PasswordHelper.HashPassword(dto.Password);
        var model = new UserModel
        {
            FullName = dto.FullName,
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = hash,
            PasswordSalt = "BCryptManaged",
            RoleId = dto.RoleId,
            DepartmentId = dto.DepartmentId,
            IsActive = dto.IsActive
        };

        var (result, error) = await _repo.InsertAsync(model);
        return result == 1
            ? ApiResponse<bool>.Ok(true, "User created successfully.")
            : ApiResponse<bool>.Fail(string.IsNullOrEmpty(error) ? "Failed to create user." : error);
    }

    public async Task<ApiResponse<bool>> UpdateAsync(UserUpdateDto dto, int? userId)
    {
        if (await _repo.EmailExistsAsync(dto.Email, dto.UserId))
            return ApiResponse<bool>.Fail("Email already exists.");

        var model = new UserModel
        {
            UserId = dto.UserId,
            FullName = dto.FullName,
            Email = dto.Email.Trim().ToLower(),
            RoleId = dto.RoleId,
            DepartmentId = dto.DepartmentId,
            IsActive = dto.IsActive
        };

        var (result, error) = await _repo.UpdateAsync(model);
        return result == 1
            ? ApiResponse<bool>.Ok(true, "User updated successfully.")
            : ApiResponse<bool>.Fail(string.IsNullOrEmpty(error) ? "Failed to update user." : error);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, int? userId)
    {
        var (result, error) = await _repo.DeleteAsync(id, userId);
        return result == 1
            ? ApiResponse<bool>.Ok(true, "User deleted successfully.")
            : ApiResponse<bool>.Fail(string.IsNullOrEmpty(error) ? "Failed to delete user." : error);
    }

    private static UserResponseDto MapToDto(UserModel u) => new()
    {
        UserId = u.UserId,
        FullName = u.FullName,
        Email = u.Email,
        RoleId = u.RoleId,
        RoleName = u.RoleName,
        DepartmentId = u.DepartmentId,
        DepartmentName = u.DepartmentName,
        IsActive = u.IsActive,
        CreatedDate = u.CreatedDate
    };
}

using ManpowerContract.Application.Common;
using ManpowerContract.Application.DTOs.Auth;
using ManpowerContract.Application.Interfaces.Repositories;
using ManpowerContract.Application.Interfaces.Services;
using ManpowerContract.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ManpowerContract.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;
    private readonly JwtHelper _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IAuthRepository repo, JwtHelper jwt, ILogger<AuthService> logger)
    {
        _repo = repo;
        _jwt = jwt;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return ApiResponse<LoginResponseDto>.Fail("Email and Password are required.");

        var user = await _repo.ValidateLoginAsync(dto.Email.Trim().ToLower());
        if (user == null)
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

        if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.Fail("Your account has been deactivated.");

        var permissions = await _repo.GetUserPermissionsAsync(user.UserId);
        var token = _jwt.GenerateToken(user, permissions);
        await _repo.UpdateLastLoginAsync(user.UserId);

        return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            RoleName = user.RoleName,
            Token = token,
            Permissions = permissions.ToList(),
            ExpiryMinutes = _jwt.GetExpiryMinutes()
        });
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return ApiResponse<bool>.Fail("New password and confirm password do not match.");

        if (!PasswordHelper.IsStrongPassword(dto.NewPassword))
            return ApiResponse<bool>.Fail("Password must be at least 8 characters with uppercase, lowercase, digit, and special character.");

        var newHash = PasswordHelper.HashPassword(dto.NewPassword);
        var result = await _repo.ChangePasswordAsync(userId, newHash, "BCryptManaged");

        return result
            ? ApiResponse<bool>.Ok(true, "Password changed successfully.")
            : ApiResponse<bool>.Fail("Failed to change password.");
    }
}

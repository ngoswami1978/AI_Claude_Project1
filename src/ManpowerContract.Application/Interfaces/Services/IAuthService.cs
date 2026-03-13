using ManpowerContract.Application.Common;
using ManpowerContract.Application.DTOs.Auth;

namespace ManpowerContract.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto);
}

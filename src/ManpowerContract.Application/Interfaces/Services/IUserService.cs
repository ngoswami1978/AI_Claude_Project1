using ManpowerContract.Application.Common;
using ManpowerContract.Application.DTOs.User;

namespace ManpowerContract.Application.Interfaces.Services;

public interface IUserService
{
    Task<ApiResponse<IEnumerable<UserResponseDto>>> SearchAsync(UserSearchDto filters);
    Task<ApiResponse<UserResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> CreateAsync(UserCreateDto dto, int? userId);
    Task<ApiResponse<bool>> UpdateAsync(UserUpdateDto dto, int? userId);
    Task<ApiResponse<bool>> DeleteAsync(int id, int? userId);
}

using ManpowerContract.Application.Models;

namespace ManpowerContract.Application.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<LoginResultModel?> ValidateLoginAsync(string email);
    Task<IEnumerable<string>> GetUserPermissionsAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, string newHash, string newSalt);
    Task UpdateLastLoginAsync(int userId);
}

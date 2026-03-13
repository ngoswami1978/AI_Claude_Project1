using ManpowerContract.Application.DTOs.User;
using ManpowerContract.Application.Models;

namespace ManpowerContract.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<UserModel>
{
    Task<IEnumerable<UserModel>> SearchAsync(UserSearchDto filters);
    Task<bool> EmailExistsAsync(string email, int excludeUserId = 0);
}

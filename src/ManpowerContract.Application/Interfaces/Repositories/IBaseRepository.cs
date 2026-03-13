namespace ManpowerContract.Application.Interfaces.Repositories;

public interface IReadRepository<T>
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
}

public interface IWriteRepository<T>
{
    Task<(int Result, string Error)> InsertAsync(T entity);
    Task<(int Result, string Error)> UpdateAsync(T entity);
    Task<(int Result, string Error)> DeleteAsync(int id, int? userId);
}

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : class { }

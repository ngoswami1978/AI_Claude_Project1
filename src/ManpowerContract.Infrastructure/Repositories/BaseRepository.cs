using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ManpowerContract.Infrastructure.Repositories;

public abstract class BaseRepository<T> where T : class
{
    protected readonly string _conn;

    protected BaseRepository(IConfiguration config)
        => _conn = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    protected abstract T MapReader(SqlDataReader reader);

    protected async Task<(int, string)> ExecuteSpAsync(string spName, Action<SqlCommand> paramSetup)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure };

        paramSetup(cmd);

        var result = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var error = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(result);
        cmd.Parameters.Add(error);

        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        return ((int)result.Value, error.Value?.ToString() ?? "");
    }

    protected async Task<List<T>> QueryAsync(string spName, Action<SqlCommand>? paramSetup = null)
    {
        var list = new List<T>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure };
        paramSetup?.Invoke(cmd);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapReader(reader));
        return list;
    }

    protected async Task<T?> QuerySingleAsync(string spName, Action<SqlCommand>? paramSetup = null)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure };
        paramSetup?.Invoke(cmd);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapReader(reader) : default;
    }
}

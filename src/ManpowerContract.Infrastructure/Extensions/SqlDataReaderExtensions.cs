using Microsoft.Data.SqlClient;

namespace ManpowerContract.Infrastructure.Extensions;

public static class SqlDataReaderExtensions
{
    public static bool HasColumn(this SqlDataReader r, string col)
        => Enumerable.Range(0, r.FieldCount)
            .Any(i => r.GetName(i).Equals(col, StringComparison.OrdinalIgnoreCase));

    public static string GetSafeString(this SqlDataReader r, string col, string def = "")
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetString(r.GetOrdinal(col)) : def;

    public static int GetSafeInt32(this SqlDataReader r, string col, int def = 0)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetInt32(r.GetOrdinal(col)) : def;

    public static int? GetSafeNullableInt32(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetInt32(r.GetOrdinal(col)) : null;

    public static decimal? GetSafeNullableDecimal(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetDecimal(r.GetOrdinal(col)) : null;

    public static DateTime GetSafeDateTime(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetDateTime(r.GetOrdinal(col)) : DateTime.MinValue;

    public static DateTime? GetSafeNullableDateTime(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetDateTime(r.GetOrdinal(col)) : null;

    public static bool GetSafeBool(this SqlDataReader r, string col, bool def = false)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetBoolean(r.GetOrdinal(col)) : def;
}

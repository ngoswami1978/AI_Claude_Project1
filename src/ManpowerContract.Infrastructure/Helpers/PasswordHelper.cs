namespace ManpowerContract.Infrastructure.Helpers;

public static class PasswordHelper
{
    public static string HashPassword(string rawPassword)
        => BCrypt.Net.BCrypt.HashPassword(rawPassword, workFactor: 12);

    public static bool VerifyPassword(string rawPassword, string storedHash)
        => BCrypt.Net.BCrypt.Verify(rawPassword, storedHash);

    public static bool IsStrongPassword(string password)
        => password.Length >= 8 &&
           password.Any(char.IsUpper) &&
           password.Any(char.IsLower) &&
           password.Any(char.IsDigit) &&
           password.Any(c => "!@#$%^&*".Contains(c));
}

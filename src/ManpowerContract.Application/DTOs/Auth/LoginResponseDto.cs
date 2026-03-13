namespace ManpowerContract.Application.DTOs.Auth;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
    public List<string> Permissions { get; set; } = new();
}

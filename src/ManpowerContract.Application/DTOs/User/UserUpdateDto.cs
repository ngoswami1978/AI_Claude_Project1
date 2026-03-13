using System.ComponentModel.DataAnnotations;

namespace ManpowerContract.Application.DTOs.User;

public class UserUpdateDto
{
    [Required] public int UserId { get; set; }
    [Required] public string FullName { get; set; } = string.Empty;
    [Required][EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public int RoleId { get; set; }
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}

using System.ComponentModel.DataAnnotations;

namespace ManpowerContract.Application.DTOs.Role;

public class RoleUpdateDto
{
    [Required] public int RoleId { get; set; }
    [Required] public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

using System.ComponentModel.DataAnnotations;

namespace ManpowerContract.Application.DTOs.Role;

public class RoleCreateDto
{
    [Required] public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

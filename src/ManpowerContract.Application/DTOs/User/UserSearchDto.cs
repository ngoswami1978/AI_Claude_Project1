namespace ManpowerContract.Application.DTOs.User;

public class UserSearchDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public int? RoleId { get; set; }
    public int? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
}

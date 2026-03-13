namespace ManpowerContract.Application.DTOs.Permission;

public class CopyPermissionDto
{
    public int TargetRoleId { get; set; }
    public int? SourceRoleId { get; set; }
    public int? SourceUserId { get; set; }
}

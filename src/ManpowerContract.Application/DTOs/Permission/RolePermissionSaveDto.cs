namespace ManpowerContract.Application.DTOs.Permission;

public class RolePermissionSaveDto
{
    public int RoleId { get; set; }
    public List<ModulePermissionDto> Permissions { get; set; } = new();
}

public class ModulePermissionDto
{
    public int ModuleId { get; set; }
    public bool CanCreate { get; set; }
    public bool CanDisable { get; set; }
    public bool CanView { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDownload { get; set; }
}

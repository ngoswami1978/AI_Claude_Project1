namespace ManpowerContract.Application.DTOs.Permission;

public class RolePermissionDto
{
    public int RolePermissionId { get; set; }
    public int ModuleId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public int? ParentModuleId { get; set; }
    public string? ParentModuleName { get; set; }
    public bool CanCreate { get; set; }
    public bool CanDisable { get; set; }
    public bool CanView { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDownload { get; set; }
}

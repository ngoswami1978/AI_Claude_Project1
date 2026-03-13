namespace ManpowerContract.Application.Models;

public class ModuleModel
{
    public int ModuleId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public int? ParentModuleId { get; set; }
    public string? IconClass { get; set; }
    public string? UrlPath { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

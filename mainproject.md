# 🚀 MAINPROJECT.MD — MASTER PROMPT
## Manpower Contract Project — Full Code Generator

---

## TECH STACK
```
Frontend  : ASP.NET Core MVC (Razor Views), jQuery, Bootstrap 5,
            DataTables.js, Select2, SweetAlert2
Backend   : ASP.NET Core Web API (separate project)
Auth      : JWT Bearer Token + HttpOnly Cookie + Session
Pattern   : Repository Pattern + Service Layer + SOLID Principles
Database  : Microsoft SQL Server (ADO.NET + Stored Procedures only)
Version   : .NET 8.0
```

---

═══════════════════════════════════════════════════════════════
## INSTRUCTIONS — FOLLOW IN EXACT ORDER
═══════════════════════════════════════════════════════════════

---

## STEP 0 — SOLUTION ARCHITECTURE (ALWAYS GENERATE THIS FIRST)

Generate ONE solution with FOUR projects:

```
MyProject.sln
├── MyProject.Application    ← Interfaces + DTOs + Common
├── MyProject.Infrastructure ← Repository + Service implementations
├── MyProject.API            ← Web API (JWT + Controllers)
└── MyProject.MVC            ← Razor Views + jQuery frontend
```

### Project References:
```
MyProject.API            → references Application + Infrastructure
MyProject.Infrastructure → references Application
MyProject.MVC            → NO reference to API (calls via HttpClient)
```

---

## STEP 1 — ANALYZE THE LAYOUT

- Carefully read the layout provided at the bottom
- Identify ALL UI elements:
  • Input fields (text, number, date, dropdown, checkbox, radio)
  • Buttons (Save, Search, Clear, Export, Delete)
  • Grids / tables (with columns, actions)
  • Tabs, modals, panels
  • Labels and their field mappings
- Identify which fields are:
  • Mandatory (mark with * in UI)
  • Read-only / auto-populated
  • Dropdowns (static or dynamic from DB)
  • Filter fields vs form fields
  • Display-only join fields (from DB joins — NOT submitted)

---

## STEP 2 — DATABASE SCRIPTS (SQL SERVER)

Generate scripts in this exact order:

### 02-A: AUTH TABLES (always generate these first — required for login)

```sql
-- MST_ROLE
CREATE TABLE [dbo].[MST_ROLE] (
    ROLE_ID         INT IDENTITY(1,1) NOT NULL,
    ROLE_NAME       NVARCHAR(100)     NOT NULL,
    DESCRIPTION     NVARCHAR(500)     NULL,
    IS_ACTIVE       BIT               NOT NULL DEFAULT 1,
    IS_DELETED      BIT               NOT NULL DEFAULT 0,
    C_USER_ID       INT               NULL,
    C_DATETIME      DATETIME          NOT NULL DEFAULT GETDATE(),
    M_USER_ID       INT               NULL,
    M_DATETIME      DATETIME          NULL,
    CONSTRAINT PK_MST_ROLE PRIMARY KEY CLUSTERED (ROLE_ID)
);

-- MST_USER
CREATE TABLE [dbo].[MST_USER] (
    USER_ID         INT IDENTITY(1,1) NOT NULL,
    FULL_NAME       NVARCHAR(200)     NOT NULL,
    EMAIL           NVARCHAR(200)     NOT NULL,
    PASSWORD_HASH   NVARCHAR(500)     NOT NULL,
    PASSWORD_SALT   NVARCHAR(500)     NOT NULL,
    ROLE_ID         INT               NOT NULL,
    IS_ACTIVE       BIT               NOT NULL DEFAULT 1,
    IS_DELETED      BIT               NOT NULL DEFAULT 0,
    LAST_LOGIN_DT   DATETIME          NULL,
    C_USER_ID       INT               NULL,
    C_DATETIME      DATETIME          NOT NULL DEFAULT GETDATE(),
    M_USER_ID       INT               NULL,
    M_DATETIME      DATETIME          NULL,
    CONSTRAINT PK_MST_USER    PRIMARY KEY CLUSTERED (USER_ID),
    CONSTRAINT FK_USER_ROLE   FOREIGN KEY (ROLE_ID)
        REFERENCES MST_ROLE(ROLE_ID),
    CONSTRAINT UQ_USER_EMAIL  UNIQUE (EMAIL)
);

-- MST_MODULE
CREATE TABLE [dbo].[MST_MODULE] (
    MODULE_ID       INT IDENTITY(1,1) NOT NULL,
    MODULE_CODE     NVARCHAR(50)      NOT NULL,
    MODULE_NAME     NVARCHAR(100)     NOT NULL,
    ICON_CLASS      NVARCHAR(100)     NULL,
    URL_PATH        NVARCHAR(200)     NULL,
    DISPLAY_ORDER   INT               NOT NULL DEFAULT 0,
    IS_ACTIVE       BIT               NOT NULL DEFAULT 1,
    CONSTRAINT PK_MST_MODULE  PRIMARY KEY CLUSTERED (MODULE_ID),
    CONSTRAINT UQ_MODULE_CODE UNIQUE (MODULE_CODE)
);

-- MST_ROLE_PERMISSION
CREATE TABLE [dbo].[MST_ROLE_PERMISSION] (
    ROLE_PERMISSION_ID INT IDENTITY(1,1) NOT NULL,
    ROLE_ID            INT               NOT NULL,
    MODULE_ID          INT               NOT NULL,
    CAN_VIEW           BIT               NOT NULL DEFAULT 0,
    CAN_ADD            BIT               NOT NULL DEFAULT 0,
    CAN_EDIT           BIT               NOT NULL DEFAULT 0,
    CAN_DELETE         BIT               NOT NULL DEFAULT 0,
    CAN_EXPORT         BIT               NOT NULL DEFAULT 0,
    C_USER_ID          INT               NULL,
    C_DATETIME         DATETIME          NOT NULL DEFAULT GETDATE(),
    M_USER_ID          INT               NULL,
    M_DATETIME         DATETIME          NULL,
    CONSTRAINT PK_MST_ROLE_PERMISSION PRIMARY KEY CLUSTERED (ROLE_PERMISSION_ID),
    CONSTRAINT FK_RP_ROLE   FOREIGN KEY (ROLE_ID)   REFERENCES MST_ROLE(ROLE_ID),
    CONSTRAINT FK_RP_MODULE FOREIGN KEY (MODULE_ID) REFERENCES MST_MODULE(MODULE_ID),
    CONSTRAINT UQ_ROLE_MODULE UNIQUE (ROLE_ID, MODULE_ID)
);

-- TRN_AUDIT_LOG
CREATE TABLE [dbo].[TRN_AUDIT_LOG] (
    LOG_ID          INT IDENTITY(1,1) NOT NULL,
    USER_ID         INT               NULL,
    MODULE          NVARCHAR(100)     NULL,
    ACTION          NVARCHAR(100)     NULL,
    DESCRIPTION     NVARCHAR(1000)    NULL,
    IP_ADDRESS      NVARCHAR(50)      NULL,
    LOG_DATETIME    DATETIME          NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_TRN_AUDIT_LOG PRIMARY KEY CLUSTERED (LOG_ID)
);
```

### 02-B: FEATURE TABLES
```
- Use NVARCHAR (not VARCHAR) for all text columns
- IDENTITY(1,1) for all PKs
- IS_DELETED BIT NOT NULL DEFAULT 0 (soft delete only)
- Audit: C_USER_ID, C_DATETIME, M_USER_ID, M_DATETIME
- FOREIGN KEY constraints with proper names
- Indexes on ALL FK columns + commonly searched columns
```

### 02-C: AUTH STORED PROCEDURES

Generate all auth SPs:
```
usp_Auth_LOGIN              → validate email/password, return user+role+permissions
usp_Auth_CHANGE_PASSWORD    → change password with old password validation
usp_Auth_RESET_PASSWORD     → admin resets user password

usp_User_INSERT             → create new user (hash password)
usp_User_UPDATE             → update user details
usp_User_DELETE             → soft delete user
usp_User_GETBYID            → get user with role name
usp_User_SEARCH             → filter users
usp_User_CHANGEPASSWORD     → change own password

usp_Role_INSERT
usp_Role_UPDATE
usp_Role_DELETE
usp_Role_GETBYID
usp_Role_SEARCH

usp_RolePermission_SAVE     → save entire permission matrix for a role
usp_RolePermission_GETBYROLE → get all module permissions for a role

usp_Lookup_Role             → dropdown: all active roles
usp_Lookup_Module           → get all modules with permissions for role
```

### 02-D: FEATURE STORED PROCEDURES
```
For each feature table generate:
- usp_[TableName]_INSERT
- usp_[TableName]_UPDATE
- usp_[TableName]_DELETE  → soft delete (IS_DELETED = 1)
- usp_[TableName]_GETBYID → with all JOINs, named columns only
- usp_[TableName]_SEARCH  → nullable params, WITH (NOLOCK)
- usp_Lookup_[TableName]  → for every dropdown

Rules for EVERY SP:
✅ SET NOCOUNT ON
✅ BEGIN TRY / BEGIN CATCH / ROLLBACK TRANSACTION
✅ WITH (NOLOCK) on all SELECT
✅ @Result INT OUTPUT (1=success, 0=fail)
✅ @ErrorMessage NVARCHAR(500) OUTPUT
✅ Parameterized only — NEVER string concatenation
✅ NEVER SELECT * — always name columns explicitly
```

### 02-E: SEED DATA
```sql
-- Seed Roles
INSERT MST_ROLE (ROLE_NAME) VALUES ('Admin'), ('Manager'), ('Viewer');

-- Seed Modules (match your feature screens)
INSERT MST_MODULE (MODULE_CODE, MODULE_NAME, ICON_CLASS, URL_PATH, DISPLAY_ORDER)
VALUES
('DASHBOARD',          'Dashboard',           'bi bi-speedometer2', '/Dashboard',         1),
('USER_MGMT',          'User Management',     'bi bi-people',       '/UserManagement',    2),
('ROLE_MGMT',          'Role Management',     'bi bi-shield-check', '/RoleManagement',    3),
('MANPOWER_CONTRACT',  'Manpower Contract',   'bi bi-file-text',    '/ManpowerContract',  4);

-- Seed Admin permissions (all true for Admin role)
INSERT MST_ROLE_PERMISSION (ROLE_ID, MODULE_ID, CAN_VIEW, CAN_ADD, CAN_EDIT, CAN_DELETE, CAN_EXPORT)
SELECT 1, MODULE_ID, 1, 1, 1, 1, 1 FROM MST_MODULE;

-- Seed Admin User (Password: Admin@123)
-- BCrypt hash generated for "Admin@123"
EXEC usp_User_INSERT @FullName='System Admin', @Email='admin@company.com',
     @RawPassword='Admin@123', @RoleId=1, @CUserId=NULL,
     @Result=@r OUTPUT, @ErrorMessage=@e OUTPUT;
```

---

## STEP 3 — APPLICATION LAYER (MyProject.Application)

### 3.1 API Response Wrapper
```csharp
public class ApiResponse<T>
{
    public bool    Success { get; set; }
    public string  Message { get; set; } = string.Empty;
    public T?      Data    { get; set; }

    public static ApiResponse<T> Ok(T data, string msg = "Success")
        => new() { Success = true,  Message = msg, Data = data };

    public static ApiResponse<T> Fail(string msg)
        => new() { Success = false, Message = msg };
}
```

### 3.2 Interfaces — Repository (SOLID: I — segregated)
```csharp
// Read-only
public interface IReadRepository<T> {
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
}

// Write-only
public interface IWriteRepository<T> {
    Task<(int Result, string Error)> InsertAsync(T entity);
    Task<(int Result, string Error)> UpdateAsync(T entity);
    Task<(int Result, string Error)> DeleteAsync(int id, int? userId);
}

// Combined
public interface IRepository<T>
    : IReadRepository<T>, IWriteRepository<T> where T : class { }

// Auth-specific
public interface IAuthRepository {
    Task<LoginResultModel?> ValidateLoginAsync(string email, string passwordHash);
    Task<IEnumerable<string>> GetUserPermissionsAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, string newHash);
    Task UpdateLastLoginAsync(int userId);
}

// User-specific
public interface IUserRepository : IRepository<UserModel> {
    Task<IEnumerable<UserModel>> SearchAsync(UserSearchDto filters);
    Task<bool> EmailExistsAsync(string email, int excludeUserId = 0);
}

// Role-specific
public interface IRoleRepository : IRepository<RoleModel> {
    Task<IEnumerable<RoleModel>> SearchAsync(RoleSearchDto filters);
    Task SavePermissionsAsync(int roleId, List<RolePermissionModel> permissions, int? userId);
    Task<IEnumerable<RolePermissionModel>> GetPermissionsByRoleAsync(int roleId);
}

// Feature-specific (replace TableName)
public interface I[TableName]Repository : IRepository<[TableName]Model> {
    Task<IEnumerable<[TableName]Model>> SearchAsync([TableName]SearchDto filters);
    Task<IEnumerable<DropdownDto>> GetGroupCompaniesAsync();
    Task<IEnumerable<PlantDropdownDto>> GetPlantsAsync(int? groupCompanyId);
}
```

### 3.3 Interfaces — Service (SOLID: I)
```csharp
public interface IAuthService {
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    string GenerateJwtToken(LoginResultModel user, IEnumerable<string> permissions);
}

public interface IUserService {
    Task<ApiResponse<IEnumerable<UserResponseDto>>> SearchAsync(UserSearchDto filters);
    Task<ApiResponse<UserResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> CreateAsync(UserCreateDto dto, int? userId);
    Task<ApiResponse<bool>> UpdateAsync(UserUpdateDto dto, int? userId);
    Task<ApiResponse<bool>> DeleteAsync(int id, int? userId);
}

public interface IRoleService {
    Task<ApiResponse<IEnumerable<RoleResponseDto>>> SearchAsync(RoleSearchDto filters);
    Task<ApiResponse<RoleResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> CreateAsync(RoleCreateDto dto, int? userId);
    Task<ApiResponse<bool>> UpdateAsync(RoleUpdateDto dto, int? userId);
    Task<ApiResponse<bool>> DeleteAsync(int id, int? userId);
    Task<ApiResponse<bool>> SavePermissionsAsync(RolePermissionSaveDto dto, int? userId);
    Task<ApiResponse<IEnumerable<RolePermissionDto>>> GetPermissionsAsync(int roleId);
}

public interface I[TableName]Service {
    Task<ApiResponse<IEnumerable<[TableName]ResponseDto>>> SearchAsync([TableName]SearchDto f);
    Task<ApiResponse<[TableName]ResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> CreateAsync([TableName]CreateDto dto, int? userId);
    Task<ApiResponse<bool>> UpdateAsync([TableName]UpdateDto dto, int? userId);
    Task<ApiResponse<bool>> DeleteAsync(int id, int? userId);
    Task<IEnumerable<DropdownDto>> GetGroupCompanyDropdownAsync();
    Task<IEnumerable<PlantDropdownDto>> GetPlantDropdownAsync(int? gcId);
}
```

### 3.4 DTOs (Auth)
```csharp
// Login
public class LoginRequestDto {
    [Required] public string Email    { get; set; }
    [Required] public string Password { get; set; }
}

public class LoginResponseDto {
    public int    UserId   { get; set; }
    public string FullName { get; set; }
    public string Email    { get; set; }
    public string RoleName { get; set; }
    public string Token    { get; set; }
    public int    ExpiryMinutes { get; set; }
    public List<string> Permissions { get; set; } = new();
}

// Change Password
public class ChangePasswordDto {
    [Required] public string CurrentPassword { get; set; }
    [Required][MinLength(8)] public string NewPassword { get; set; }
    [Required] public string ConfirmPassword { get; set; }
}

// User DTOs
public class UserCreateDto {
    [Required] public string FullName { get; set; }
    [Required][EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public int    RoleId   { get; set; }
    public bool IsActive { get; set; } = true;
}

// Role Permission DTO
public class RolePermissionSaveDto {
    public int RoleId { get; set; }
    public List<ModulePermissionDto> Permissions { get; set; } = new();
}

public class ModulePermissionDto {
    public int  ModuleId  { get; set; }
    public bool CanView   { get; set; }
    public bool CanAdd    { get; set; }
    public bool CanEdit   { get; set; }
    public bool CanDelete { get; set; }
    public bool CanExport { get; set; }
}
```

---

## STEP 4 — INFRASTRUCTURE LAYER (MyProject.Infrastructure)

### 4.1 BaseRepository<T> (SOLID: O — Open for extension)
```csharp
public abstract class BaseRepository<T> where T : class
{
    protected readonly string _conn;

    protected BaseRepository(IConfiguration config)
        => _conn = config.GetConnectionString("DefaultConnection")!;

    protected abstract T MapReader(SqlDataReader reader);

    protected async Task<(int, string)> ExecuteSpAsync(
        string spName, Action<SqlCommand> paramSetup)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(spName, con)
            { CommandType = CommandType.StoredProcedure };
        paramSetup(cmd);
        var result = new SqlParameter("@Result",       SqlDbType.Int)
            { Direction = ParameterDirection.Output };
        var error  = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 500)
            { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(result);
        cmd.Parameters.Add(error);
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return ((int)result.Value, error.Value?.ToString() ?? "");
    }

    protected async Task<List<T>> QueryAsync(
        string spName, Action<SqlCommand>? paramSetup = null)
    {
        var list = new List<T>();
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(spName, con)
            { CommandType = CommandType.StoredProcedure };
        paramSetup?.Invoke(cmd);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapReader(reader));
        return list;
    }
}
```

### 4.2 AuthRepository
```csharp
// Implements IAuthRepository
// Calls: usp_Auth_LOGIN → returns user info + permissions list
// Password verification: BCrypt.Net.BCrypt.Verify(rawPassword, storedHash)
// Returns LoginResultModel with UserId, Email, FullName, RoleId, RoleName
```

### 4.3 AuthService
```csharp
// SOLID: S — only auth business logic
public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;
    private readonly IConfiguration  _config;

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        // 1. Basic validation
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
            return ApiResponse<LoginResponseDto>.Fail("Email and Password are required.");

        // 2. Get user from DB (SP validates credentials)
        var user = await _repo.ValidateLoginAsync(dto.Email.Trim().ToLower());
        if (user == null)
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

        // 3. Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

        // 4. Check if active
        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.Fail("Your account has been deactivated.");

        // 5. Get permissions
        var permissions = await _repo.GetUserPermissionsAsync(user.UserId);

        // 6. Generate JWT
        var token = GenerateJwtToken(user, permissions);

        // 7. Update last login
        await _repo.UpdateLastLoginAsync(user.UserId);

        return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto {
            UserId      = user.UserId,
            FullName    = user.FullName,
            Email       = user.Email,
            RoleName    = user.RoleName,
            Token       = token,
            Permissions = permissions.ToList(),
            ExpiryMinutes = int.Parse(_config["JwtSettings:ExpiryMinutes"]!)
        });
    }

    public string GenerateJwtToken(LoginResultModel user, IEnumerable<string> permissions)
    {
        var key   = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email,          user.Email),
            new(ClaimTypes.Name,           user.FullName),
            new("RoleId",                  user.RoleId.ToString()),
            new(ClaimTypes.Role,           user.RoleName),
            new("Permissions", JsonSerializer.Serialize(permissions))
        };

        var token = new JwtSecurityToken(
            issuer:             _config["JwtSettings:Issuer"],
            audience:           _config["JwtSettings:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(
                                    double.Parse(_config["JwtSettings:ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 4.4 PasswordHelper
```csharp
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
```

### 4.5 Feature Repository & Service
```csharp
// [TableName]Repository : BaseRepository<[TableName]Model>, I[TableName]Repository
// - InsertAsync  → calls usp_[TableName]_INSERT
// - UpdateAsync  → calls usp_[TableName]_UPDATE
// - DeleteAsync  → calls usp_[TableName]_DELETE
// - GetByIdAsync → calls usp_[TableName]_GETBYID
// - SearchAsync  → calls usp_[TableName]_SEARCH
// - Lookups      → usp_Lookup_[TableName]

// [TableName]Service : I[TableName]Service
// Business rules:
// - EndDate must be after StartDate
// - Amount must not be negative
// - Duplicate code check
// - Map DTO ↔ Model
// - Return ApiResponse<T>
```

---

## STEP 5 — API PROJECT (MyProject.API)

### 5.1 AuthController
```csharp
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid) return Ok(ApiResponse<bool>.Fail("Invalid input."));
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _authService.ChangePasswordAsync(userId, dto);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(ApiResponse<object>.Ok(new {
            userId   = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            email    = User.FindFirst(ClaimTypes.Email)?.Value,
            fullName = User.FindFirst(ClaimTypes.Name)?.Value,
            role     = User.FindFirst(ClaimTypes.Role)?.Value
        }));
    }
}
```

### 5.2 UserController, RoleController, RolePermissionController
```csharp
// UserController — CRUD for users
[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UserController : ControllerBase { ... }

// RoleController — CRUD for roles
[ApiController]
[Route("api/v1/roles")]
[Authorize]
public class RoleController : ControllerBase { ... }

// RolePermissionController — save/get permission matrix
[ApiController]
[Route("api/v1/role-permissions")]
[Authorize]
public class RolePermissionController : ControllerBase {
    [HttpGet("{roleId}")]          // get permissions for role
    [HttpPost]                     // save permission matrix
}
```

### 5.3 Feature Controller (one per screen)
```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class [TableName]Controller : ControllerBase
{
    [HttpGet("search")]        // GET with filter params
    [HttpGet("{id}")]          // GET single by ID
    [HttpPost]                 // CREATE
    [HttpPut("{id}")]          // UPDATE
    [HttpDelete("{id}")]       // SOFT DELETE
    [HttpGet("lookup/group-companies")]
    [HttpGet("lookup/plants")]          // returns { value, text, country }
    [HttpGet("lookup/departments")]
    [HttpGet("lookup/currencies")]
    [HttpGet("export-excel")]           // ClosedXML export
}
```

### 5.4 Exception Middleware
```csharp
public class ExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (UnauthorizedAccessException ex) {
            _logger.LogWarning(ex, "Unauthorized");
            await WriteJson(context, 401, "Unauthorized.");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);
            await WriteJson(context, 500, "An unexpected error occurred.");
            // NEVER expose ex.Message to client
        }
    }
}
```

### 5.5 API Program.cs
```csharp
// Register repositories (SOLID: D)
builder.Services.AddScoped<IAuthRepository,       AuthRepository>();
builder.Services.AddScoped<IUserRepository,       UserRepository>();
builder.Services.AddScoped<IRoleRepository,       RoleRepository>();
builder.Services.AddScoped<I[TableName]Repository,[TableName]Repository>();

// Register services (SOLID: D)
builder.Services.AddScoped<IAuthService,          AuthService>();
builder.Services.AddScoped<IUserService,          UserService>();
builder.Services.AddScoped<IRoleService,          RoleService>();
builder.Services.AddScoped<I[TableName]Service,   [TableName]Service>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience            = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

// CORS for MVC
builder.Services.AddCors(o => o.AddPolicy("MVC", p =>
    p.WithOrigins(builder.Configuration["AllowedOrigins"]!
        .Split(',', StringSplitOptions.RemoveEmptyEntries))
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));

builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// Middleware pipeline
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("MVC");
app.UseAuthentication();
app.UseAuthorization();
```

---

## STEP 6 — MVC PROJECT (MyProject.MVC)

### 6.1 Login Screen (Views/Account/Login.cshtml)

```
LAYOUT (Extracted from Manpower Contract Management Login Screenshot):
- Full-page split layout (no navbar/sidebar — standalone page)
- LEFT PANEL (40%):
    - Background: Solid crimson red (#db2128)
    - Gold accent vertical bar (#FFB300) next to title
    - Title: "Manpower Contract Management" (white text, bold, 28px)
    - No illustration — clean solid color
- RIGHT PANEL (60%):
    - White background (#FFFFFF)
    - Decorative gray circles (top-right & bottom-right, #F0F0F0)
    - Heading: "Manpower Contract Management" (dark, bold)
    - Language dropdown: "English (United States)"
    - Username input (user icon, border #CED4DA, focus bg #E3F2FD)
    - Password input (lock icon + eye toggle for show/hide)
    - "Remember Me" checkbox + "Forgot Password?" link (teal #009688)
    - LOGIN button (full-width, teal #009688, white text, 600 weight, uppercase)
    - Footer tagline: "Proud to be the part of." (crimson red text #db2128)
- RESPONSIVE: Left panel hidden on mobile (< 768px)

CSS ROOT VARIABLES (EXTRACTED — apply to entire project):
:root {
    /* ═══ PRIMARY BRAND — Crimson Red ═══ */
    --primary-color:          #db2128;   /* Brand red — left panel, accents */
    --primary-dark:           #A31D28;   /* Darker red — hover states */
    --primary-light:          #E8434F;   /* Lighter red — highlights */

    /* ═══ CTA / ACTION — Teal Green ═══ */
    --teal-color:             #009688;   /* Teal — Login btn, Save btn, links */
    --teal-dark:              #00796B;   /* Dark teal — button hover */
    --teal-light:             #4DB6AC;   /* Light teal — highlights */

    /* ═══ ACCENT — Gold (Title bar accent) ═══ */
    --accent-gold:            #FFB300;   /* Gold accent bar next to title */

    /* ═══ LAYOUT ═══ */
    --login-left-bg:          #db2128;   /* Solid crimson red left panel */
    --sidebar-bg:             #1A1A2E;   /* Dark charcoal sidebar */
    --sidebar-hover:          #2A2A42;   /* Sidebar item hover */
    --sidebar-active:         #db2128;   /* Active item — red highlight */
    --navbar-bg:              #FFFFFF;   /* White top navbar */
    --navbar-border:          #E0E0E0;   /* Navbar bottom border */
    --page-bg:                #F0F2F5;   /* Light gray page bg */
    --card-bg:                #FFFFFF;   /* White cards */

    /* ═══ TEXT ═══ */
    --text-on-dark:           #FFFFFF;   /* White on dark/red bg */
    --text-on-dark-muted:     #E0E0E0;   /* Muted on dark bg */
    --text-primary:           #212529;   /* Body text */
    --text-secondary:         #6C757D;   /* Muted text */
    --text-heading:           #212529;   /* Headings */
    --text-brand-red:         #db2128;   /* Red branded text */

    /* ═══ TABLE ═══ */
    --table-header-bg:        #db2128;   /* Crimson red header */
    --table-header-text:      #FFFFFF;   /* White header text */
    --table-row-hover:        #FFF3F4;   /* Light pink hover */

    /* ═══ FORM ELEMENTS ═══ */
    --input-border:           #CED4DA;
    --input-focus-border:     #009688;   /* Teal focus border */
    --input-focus-bg:         #E3F2FD;   /* Light blue focus background */
    --input-focus-shadow:     rgba(0,150,136,0.25);

    /* ═══ BUTTONS ═══ */
    --btn-primary-bg:         #009688;   /* Teal — Login, Save, Submit */
    --btn-primary-hover:      #00796B;   /* Dark teal hover */
    --btn-secondary-bg:       #db2128;   /* Red — branded secondary */
    --btn-outline-color:      #009688;   /* Teal outline */
    --btn-danger-bg:          #DC3545;   /* Red — delete */

    /* ═══ MODAL ═══ */
    --modal-header-bg:        #db2128;   /* Red modal header */
    --modal-header-text:      #FFFFFF;

    /* ═══ DECORATIVE ═══ */
    --circle-decoration:      #F0F0F0;   /* Gray decorative circles */
}
```

### 6.2 AccountController (MVC)
```csharp
public class AccountController : Controller
{
    [HttpGet]  public IActionResult Login(string? returnUrl) { ... }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        // 1. Call API: POST /api/v1/auth/login
        var result = await _api.PostAsync<LoginResponseDto>("api/v1/auth/login",
            new { email = model.Email, password = model.Password });

        if (result?.Success != true) {
            ModelState.AddModelError("", result?.Message ?? "Login failed.");
            return View(model);
        }

        // 2. Store JWT in HttpOnly cookie
        Response.Cookies.Append(SessionKeys.AuthToken, result.Data!.Token,
            new CookieOptions {
                HttpOnly = true,
                Secure   = true,
                SameSite = SameSiteMode.Strict,
                Expires  = DateTimeOffset.UtcNow.AddMinutes(result.Data.ExpiryMinutes)
            });

        // 3. Store user info in Session
        HttpContext.Session.SetInt32(SessionKeys.UserId,      result.Data.UserId);
        HttpContext.Session.SetString(SessionKeys.UserName,   result.Data.FullName);
        HttpContext.Session.SetString(SessionKeys.Email,      result.Data.Email);
        HttpContext.Session.SetString(SessionKeys.RoleName,   result.Data.RoleName);
        HttpContext.Session.SetString(SessionKeys.Permissions,
            JsonSerializer.Serialize(result.Data.Permissions));

        return Redirect(returnUrl ?? "/Dashboard");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        // Clear session
        HttpContext.Session.Clear();
        // Expire cookie
        Response.Cookies.Delete(SessionKeys.AuthToken);
        return RedirectToAction("Login");
    }
}
```

### 6.3 Authentication Filter (applies to ALL MVC controllers)
```csharp
public class AuthenticationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Skip Login page
        var skipAuth = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>().Any();
        if (skipAuth) return;

        // Check session
        var userId = context.HttpContext.Session.GetInt32(SessionKeys.UserId);
        if (userId == null || userId == 0)
        {
            var returnUrl = context.HttpContext.Request.Path;
            context.Result = new RedirectResult($"/Login?returnUrl={returnUrl}");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
```

### 6.4 Permission Filter
```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string module, string action)
        : base(typeof(PermissionAuthFilter))
    {
        Arguments = new object[] { module, action };
    }
}

public class PermissionAuthFilter : IAuthorizationFilter
{
    private readonly string _module;
    private readonly string _action;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permsJson = context.HttpContext.Session
            .GetString(SessionKeys.Permissions);

        if (string.IsNullOrEmpty(permsJson)) {
            context.Result = new RedirectResult("/Unauthorized");
            return;
        }

        var perms    = JsonSerializer.Deserialize<List<string>>(permsJson) ?? new();
        var required = $"{_module}.{_action}";

        if (!perms.Contains(required))
            context.Result = new RedirectResult("/Unauthorized");
    }
}
```

### 6.5 ApiClientService (MVC calls API)
```csharp
// Attaches JWT from cookie to every API call
// GetAsync<T>, PostAsync<T>, PutAsync<T>, DeleteAsync<T>
// All return ApiResponse<T>
// Logs errors with ILogger — never exposes to UI

private HttpClient CreateClient()
{
    var client = _factory.CreateClient("MyProjectAPI");
    var token  = _httpContextAccessor.HttpContext?
                     .Request.Cookies[SessionKeys.AuthToken];
    if (!string.IsNullOrEmpty(token))
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    return client;
}
```

### 6.6 Shared Layout (Views/Shared/_Layout.cshtml)
```
LAYOUT STRUCTURE:
┌─────────────────────────────────────────┐
│           TOP NAVBAR                    │
│  [Logo] [App Name]    [Username] [Logout]│
├──────────┬──────────────────────────────┤
│          │                              │
│ SIDEBAR  │      MAIN CONTENT AREA       │
│          │   (@RenderBody())            │
│ - Menu 1 │                              │
│ - Menu 2 │                              │
│ - Menu 3 │                              │
│          │                              │
│(hidden if│                              │
│no perm)  │                              │
├──────────┴──────────────────────────────┤
│              FOOTER                     │
└─────────────────────────────────────────┘

Rules:
✅ Sidebar menu items HIDDEN if user has no View permission
✅ Active menu item highlighted
✅ Username and Role shown in navbar
✅ Logout button posts to /Account/Logout
✅ Breadcrumb inside each page
✅ Anti-forgery token in layout (for all forms)
```

### 6.7 User Management Screen (Views/UserManagement/Index.cshtml)
```
FILTER PANEL:
- Search by Name / Email / Role / Status

GRID COLUMNS:
- #  | Full Name | Email | Role | Status | Created Date | Actions

MODAL FORM — Add/Edit User:
- Full Name *
- Email *
- Password * (Add only — masked)
- Role * (dropdown)
- Is Active (checkbox)

ACTIONS:
- Add New User
- Edit (pencil icon)
- Delete (trash icon — soft delete)
- Reset Password (key icon)
```

### 6.8 Role Management Screen (Views/RoleManagement/Index.cshtml)
```
GRID COLUMNS:
- # | Role Name | Description | Status | Actions

MODAL FORM — Add/Edit Role:
- Role Name *
- Description
- Is Active
```

### 6.9 Permission Matrix Screen (Views/RolePermission/Index.cshtml)
```
LAYOUT:
- Select Role (dropdown at top)
- Matrix Table:

  MODULE          | VIEW | ADD | EDIT | DELETE | EXPORT
  ─────────────────────────────────────────────────────
  Dashboard       | ☑   | ☐  | ☐   | ☐     | ☐
  User Management | ☑   | ☑  | ☑   | ☑     | ☑
  Role Management | ☑   | ☑  | ☑   | ☑     | ☑
  Manpower Cont.  | ☑   | ☑  | ☑   | ☐     | ☑
  ─────────────────────────────────────────────────────
  [Select All Row] checkbox per module

BUTTONS: Save Permissions | Reset

JS:
- On role change → load permissions via AJAX → populate checkboxes
- Select All → check all permissions for that module
- Save → POST matrix as JSON → show toast
```

### 6.10 MVC Program.cs
```csharp
// Session
builder.Services.AddSession(options => {
    options.IdleTimeout        = TimeSpan.FromMinutes(
        builder.Configuration.GetValue<int>("SessionSettings:TimeoutMinutes"));
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name        = builder.Configuration["SessionSettings:CookieName"];
});

// HttpClient for API calls
builder.Services.AddHttpClient("MyProjectAPI", client => {
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// DI (SOLID: D)
builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddHttpContextAccessor();

// Global auth filter (applies to all controllers)
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add<AuthenticationFilter>();
})
.AddNewtonsoftJson(options => {
    options.SerializerSettings.ContractResolver =
        new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
});

// Anti-forgery
builder.Services.AddAntiforgery(options =>
    options.HeaderName = "RequestVerificationToken");

// Global error handling
app.UseExceptionHandler("/Error/Index");
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
```

---

## STEP 7 — FRONTEND CODE

### 7.1 Login Page JS (wwwroot/js/account.js)
```javascript
// Password show/hide toggle
$('#togglePassword').on('click', function() {
    var input = $('#Password');
    var icon  = $(this).find('i');
    if (input.attr('type') === 'password') {
        input.attr('type', 'text');
        icon.removeClass('bi-eye').addClass('bi-eye-slash');
    } else {
        input.attr('type', 'password');
        icon.removeClass('bi-eye-slash').addClass('bi-eye');
    }
});

// Login form submit with loader
$('#loginForm').on('submit', function() {
    ShowLoginLoader(true);
});

function ShowLoginLoader(show) {
    $('#loginBtn').prop('disabled', show)
        .html(show
            ? '<span class="spinner-border spinner-border-sm me-2"></span>Logging in...'
            : '<i class="bi bi-box-arrow-in-right me-2"></i>Login');
}
```

### 7.2 Role Permission JS (wwwroot/js/role-permission.js)
```javascript
// Load permissions when role changes
$('#roleSelect').on('change', function() {
    var roleId = $(this).val();
    if (!roleId) { ClearMatrix(); return; }

    ShowLoader(true);
    $.get('/RolePermission/GetPermissions', { roleId: roleId }, function(res) {
        ShowLoader(false);
        if (!res.success) { ShowToast(res.message, 'error'); return; }
        PopulateMatrix(res.data);
    });
});

function PopulateMatrix(permissions) {
    permissions.forEach(function(perm) {
        var prefix = '#module_' + perm.moduleId;
        $(prefix + '_view').prop('checked',   perm.canView);
        $(prefix + '_add').prop('checked',    perm.canAdd);
        $(prefix + '_edit').prop('checked',   perm.canEdit);
        $(prefix + '_delete').prop('checked', perm.canDelete);
        $(prefix + '_export').prop('checked', perm.canExport);
    });
}

function SavePermissions() {
    var roleId = $('#roleSelect').val();
    if (!roleId) { ShowToast('Please select a Role.', 'warning'); return; }

    var permissions = [];
    $('.module-row').each(function() {
        var moduleId = $(this).data('moduleid');
        permissions.push({
            moduleId:  moduleId,
            canView:   $(this).find('[data-action=view]').is(':checked'),
            canAdd:    $(this).find('[data-action=add]').is(':checked'),
            canEdit:   $(this).find('[data-action=edit]').is(':checked'),
            canDelete: $(this).find('[data-action=delete]').is(':checked'),
            canExport: $(this).find('[data-action=export]').is(':checked')
        });
    });

    ShowLoader(true);
    $.ajax({
        url: '/RolePermission/Save',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ roleId: parseInt(roleId), permissions: permissions }),
        headers: { 'RequestVerificationToken': csrfToken },
        success: function(res) {
            ShowLoader(false);
            ShowToast(res.message, res.success ? 'success' : 'error');
        },
        error: function() {
            ShowLoader(false);
            ShowToast('Failed to save permissions.', 'error');
        }
    });
}
```

### 7.3 Feature JS (wwwroot/js/[feature].js)
```javascript
// Standard pattern — same as original prompt PLUS:
// - Check permission before showing Add/Edit/Delete buttons
// - csrfToken initialized at top
// - All AJAX shows/hides loader in success AND error
// - SweetAlert2 for all confirmations
// - Cascading dropdowns with data-* auto-fill
// - Column search in grid second header row
```

---

## STEP 8 — GITLAB CI/CD PIPELINE (.gitlab-ci.yml)

```yaml
stages:
  - validate
  - build
  - test
  - deploy-db
  - deploy-api
  - deploy-mvc

variables:
  DOTNET_VERSION: "8.0"

validate-sqlproj:
  stage: validate
  image: alpine
  script:
    - |
      SQLPROJ=$(find . -name "*.sqlproj" | head -1)
      DUPES=$(grep '<Build Include' $SQLPROJ | sort | uniq -d)
      if [ -n "$DUPES" ]; then
        echo "Duplicate Build entries found:"; echo "$DUPES"; exit 1
      fi
      echo "Validation passed."

build-api:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:8.0
  cache:
    key: nuget-$CI_COMMIT_REF_SLUG
    paths: [.nuget/]
  script:
    - dotnet restore MyProject.sln
    - dotnet build MyProject.sln -c Release --no-restore
    - dotnet publish src/MyProject.API -c Release -o publish/api
    - dotnet publish src/MyProject.MVC -c Release -o publish/mvc
  artifacts:
    paths: [publish/]
    expire_in: 2 hours

unit-tests:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:8.0
  script:
    - dotnet test tests/ --logger "junit;LogFilePath=results.xml"
  artifacts:
    reports:
      junit: results.xml

deploy-database:
  stage: deploy-db
  image: mcr.microsoft.com/mssql-tools
  script:
    - |
      for SCRIPT in 01_CreateTables 02_Auth_Tables \
                    03_StoredProcedures 04_Auth_StoredProcedures \
                    05_SeedData; do
        echo "Running $SCRIPT.sql..."
        /opt/mssql-tools/bin/sqlcmd \
          -S "$DB_SERVER" -U "$DB_USER" -P "$DB_PASSWORD" \
          -d "$DB_NAME" -b -i "Database/${SCRIPT}.sql"
        echo "$SCRIPT done."
      done
  only: [main, staging]

deploy-api:
  stage: deploy-api
  needs: [build-api, deploy-database]
  script:
    - rsync -avz publish/api/ $DEPLOY_USER@$DEPLOY_SERVER:/var/www/api/
  only: [main]

deploy-mvc:
  stage: deploy-mvc
  needs: [build-api]
  script:
    - rsync -avz publish/mvc/ $DEPLOY_USER@$DEPLOY_SERVER:/var/www/mvc/
  only: [main]
```

---

## STEP 9 — COMMON MISTAKES — NEVER DO THESE

```
❌ MISTAKE 1 — Hard-coded DB connection in code
GOOD: Always from appsettings.json → IConfiguration

❌ MISTAKE 2 — Exposing exception message to UI
BAD:  return Ok(new { message = ex.Message });
GOOD: _logger.LogError(ex, ...); return Ok(Fail("Unexpected error."));

❌ MISTAKE 3 — MVC directly queries database
BAD:  MVC Controller → Repository → DB
GOOD: MVC Controller → ApiClientService → API → Repository → DB

❌ MISTAKE 4 — Storing JWT in localStorage (XSS risk)
GOOD: Always HttpOnly Cookie

❌ MISTAKE 5 — No permission check on Save/Delete actions
GOOD: [HasPermission("ModuleName", "Edit")] on action

❌ MISTAKE 6 — Session not cleared on logout
GOOD: HttpContext.Session.Clear(); + Response.Cookies.Delete("AuthToken");

❌ MISTAKE 7 — [ValidateNever] missing on display-only fields in ViewModel
GOOD: [ValidateNever] public string PlantName { get; set; }

❌ MISTAKE 8 — Subclassing SelectListItem
GOOD: Use Dictionary<string, string> for extra dropdown data

❌ MISTAKE 9 — No CSRF token on AJAX POST
GOOD: headers: { 'RequestVerificationToken': csrfToken }

❌ MISTAKE 10 — Duplicate Build entries in .sqlproj
GOOD: Each file listed exactly ONCE in ItemGroup
```

---

## STEP 10 — OUTPUT FORMAT

For each file output in this exact format:

```
📄 FILE: src/MyProject.API/Controllers/AuthController.cs
─────────────────────────────────────────────────────────
[full code here — no summaries, no placeholders]
```

Generate ALL files in the folder structure.
Do NOT skip any file. Do NOT summarize — write FULL code.

After all files provide:

```
✅ SETUP INSTRUCTIONS
Step 1: Update appsettings.json (API + MVC) — connection string + JWT secret
Step 2: Run SQL in order:
        01_CreateTables.sql
        02_Auth_Tables.sql
        03_StoredProcedures.sql
        04_Auth_StoredProcedures.sql
        05_SeedData.sql
Step 3: dotnet run --project src/MyProject.API   → https://localhost:5001
Step 4: dotnet run --project src/MyProject.MVC   → https://localhost:5000
Step 5: Login: admin@company.com / Admin@123
Step 6: Go to Role Management → set permissions
Step 7: Create additional users if needed
```

---

═══════════════════════════════════════════════════════════
## NOW — HERE IS MY SCREEN LAYOUT
═══════════════════════════════════════════════════════════

> ⚠️ PASTE SCREEN SPECS BELOW BEFORE SENDING TO AI

**Screen Name:**
**Module:**
**Primary Table Name:**
**Related / Lookup Tables:**
**Grid Columns to show:**
**Mandatory Fields:**
**Auto-filled Fields** (field ← filled from):
**Cascading Dropdowns** (parent → child):
**Special Business Rules:**
**SQL Server Version:**
**.NET Version:** 8.0

> ⚠️ PASTE SCREENSHOT OR EXCEL CONTENT BELOW:

[ATTACH SCREENSHOT OR PASTE EXCEL CONTENT HERE]

> ⚠️ COMPANY THEME SCREENSHOT (for Login + Layout colors):

[ATTACH COMPANY BRANDING SCREENSHOT HERE]

---

Based on the layout and theme above, generate ALL files
following every instruction above. Do not skip any file.

---

*mainproject.md | Manpower Contract Project | 2026*

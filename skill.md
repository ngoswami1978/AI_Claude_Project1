# 🛠️ SKILL.MD
## Technical Skills, Patterns & Standards
## Manpower Contract Project

---

## 🏗️ ARCHITECTURE OVERVIEW

```
┌─────────────────────────────────────────────────────┐
│                   BROWSER / CLIENT                  │
│         (Razor Views + jQuery + Bootstrap 5)        │
└──────────────────────┬──────────────────────────────┘
                       │ HTTP (AJAX + Form Posts)
                       │ JWT Token in HttpOnly Cookie
┌──────────────────────▼──────────────────────────────┐
│              MyProject.MVC (Port 5000)              │
│   Controllers → ApiClientService → HttpClient       │
│   Session Management → Anti-forgery → Auth Filter   │
└──────────────────────┬──────────────────────────────┘
                       │ HTTP (Internal API Calls)
                       │ Bearer Token in Header
┌──────────────────────▼──────────────────────────────┐
│              MyProject.API (Port 5001)              │
│   Controllers → Service Layer → Repository Layer    │
│   JWT Validation → Global Exception Handler         │
└──────────────────────┬──────────────────────────────┘
                       │ ADO.NET (SqlConnection)
                       │ Stored Procedures only
┌──────────────────────▼──────────────────────────────┐
│           Microsoft SQL Server Database             │
│   Tables + Stored Procedures + Views + Functions    │
└─────────────────────────────────────────────────────┘
```

---

## 🧱 SOLID PRINCIPLES (ENFORCE IN EVERY FILE)

### S — Single Responsibility
```
✅ Controller     → ONLY handles HTTP routing + calls service
✅ Service        → ONLY contains business logic + validation
✅ Repository     → ONLY handles DB calls (ADO.NET / SPs)
✅ Model/Entity   → ONLY holds data shape (no logic)
✅ ApiClientService → ONLY handles HTTP calls to API
✅ Middleware     → ONLY handles cross-cutting concerns
```

### O — Open/Closed
```
✅ IBaseRepository<T>     → extend for each entity, never modify base
✅ IBaseService<T>         → extend for each service
✅ Custom attributes       → extend for new permissions, never change existing
✅ Middleware pipeline     → add new middleware, never edit ExceptionMiddleware
```

### L — Liskov Substitution
```
✅ Any class implementing IRepository<T> must fully satisfy the contract
✅ TrnRecordRepository : BaseRepository<TrnRecord>
   → must work anywhere IRepository<TrnRecord> is expected
✅ Never partial-implement an interface (use default throw = violation)
```

### I — Interface Segregation
```
✅ IReadRepository<T>    → GetById, GetAll, Search
✅ IWriteRepository<T>   → Insert, Update, Delete
✅ IRepository<T>        → inherits both (use only when both needed)
✅ IAuthService          → Login, Logout, RefreshToken (separate from business)
✅ IPermissionService    → CheckPermission, GetUserPermissions (separate)
```

### D — Dependency Inversion
```
✅ Controllers depend on IService (interface), never on Service (class)
✅ Services depend on IRepository (interface), never on Repository (class)
✅ All registered in Program.cs:
   builder.Services.AddScoped<IAuthRepository, AuthRepository>();
   builder.Services.AddScoped<IAuthService, AuthService>();
✅ NEVER: var svc = new AuthService();  ← hard dependency
✅ ALWAYS: constructor injection via interface
```

---

## 🔐 AUTHENTICATION & AUTHORIZATION

### JWT Token Flow
```csharp
// API generates JWT on successful login
var token = new JwtSecurityToken(
    issuer:   _config["JwtSettings:Issuer"],
    audience: _config["JwtSettings:Audience"],
    claims:   claims,              // UserId, Email, Role, Permissions
    expires:  DateTime.UtcNow.AddMinutes(expiryMinutes),
    signingCredentials: creds
);

// Claims inside token:
// - UserId       (int)
// - Email        (string)
// - RoleId       (int)
// - RoleName     (string)
// - Permissions  (JSON array: ["ManpowerContract.View","ManpowerContract.Add"])
```

### Cookie-based Session (MVC Side)
```csharp
// Store JWT in HttpOnly cookie (not accessible by JS — secure)
Response.Cookies.Append("AuthToken", token, new CookieOptions {
    HttpOnly  = true,
    Secure    = true,
    SameSite  = SameSiteMode.Strict,
    Expires   = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
});

// Also store user info in Session
HttpContext.Session.SetString("UserEmail", loginResult.Email);
HttpContext.Session.SetString("UserName",  loginResult.FullName);
HttpContext.Session.SetInt32("UserId",     loginResult.UserId);
HttpContext.Session.SetString("RoleName",  loginResult.RoleName);
```

### Permission Attribute (Custom)
```csharp
// Usage on any controller action:
[HasPermission("ManpowerContract", "Edit")]
public async Task<IActionResult> Save([FromBody] RecordDto dto) { }

// Implementation:
public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string module, string action)
        : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { module, action };
    }
}

public class PermissionFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permissions = context.HttpContext.Session
            .GetString("Permissions");
        var required = $"{_module}.{_action}";

        if (!permissions?.Contains(required) ?? true)
        {
            context.Result = new RedirectResult("/Unauthorized");
        }
    }
}
```

---

## 🛡️ GLOBAL EXCEPTION HANDLING

### API — ExceptionMiddleware
```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteResponse(context, 401, "Unauthorized.");
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            await WriteResponse(context, 400, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteResponse(context, 500,
                "An unexpected error occurred. Please try again.");
            // NEVER expose ex.Message to client in production
        }
    }

    private static async Task WriteResponse(
        HttpContext ctx, int statusCode, string message)
    {
        ctx.Response.StatusCode  = statusCode;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new {
            success = false,
            message = message,
            data    = (object?)null
        });
    }
}
```

### MVC — Global Error Pages
```csharp
// Program.cs
app.UseExceptionHandler("/Error/Index");
app.UseStatusCodePagesWithReExecute("/Error/{0}");

// ErrorController.cs
public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        return statusCode switch
        {
            401 => View("Unauthorized"),
            403 => View("Forbidden"),
            404 => View("NotFound"),
            _   => View("ServerError")
        };
    }
}
```

---

## 📦 REPOSITORY PATTERN (ADO.NET)

### Base Repository Interface
```csharp
public interface IReadRepository<T>
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
}

public interface IWriteRepository<T>
{
    Task<(int Result, string Error)> InsertAsync(T entity);
    Task<(int Result, string Error)> UpdateAsync(T entity);
    Task<(int Result, string Error)> DeleteAsync(int id, int? userId);
}

public interface IRepository<T>
    : IReadRepository<T>, IWriteRepository<T> where T : class { }
```

### Base Repository Implementation
```csharp
public abstract class BaseRepository<T> where T : class
{
    protected readonly string _conn;

    protected BaseRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string missing.");
    }

    protected abstract T MapReader(SqlDataReader reader);

    // Shared soft delete — all repos inherit
    protected async Task<(int, string)> SoftDeleteAsync(
        string spName, int id, int? userId)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(spName, con)
            { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@MUserId", (object?)userId ?? DBNull.Value);

        var result = new SqlParameter("@Result", SqlDbType.Int)
            { Direction = ParameterDirection.Output };
        var error  = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 500)
            { Direction = ParameterDirection.Output };

        cmd.Parameters.Add(result);
        cmd.Parameters.Add(error);

        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        return ((int)result.Value, error.Value?.ToString() ?? "");
    }
}
```

### SqlDataReader Extension Methods
```csharp
public static class SqlDataReaderExtensions
{
    public static bool     HasColumn(this SqlDataReader r, string col)
        => Enumerable.Range(0, r.FieldCount)
            .Any(i => r.GetName(i).Equals(col, StringComparison.OrdinalIgnoreCase));

    public static string   GetSafeString(this SqlDataReader r, string col, string def = "")
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetString(r.GetOrdinal(col)) : def;

    public static int      GetSafeInt32(this SqlDataReader r, string col, int def = 0)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetInt32(r.GetOrdinal(col)) : def;

    public static int?     GetSafeNullableInt32(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetInt32(r.GetOrdinal(col)) : null;

    public static decimal? GetSafeNullableDecimal(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetDecimal(r.GetOrdinal(col)) : null;

    public static DateTime GetSafeDateTime(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetDateTime(r.GetOrdinal(col)) : DateTime.MinValue;

    public static DateTime? GetSafeNullableDateTime(this SqlDataReader r, string col)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetDateTime(r.GetOrdinal(col)) : null;

    public static bool     GetSafeBool(this SqlDataReader r, string col, bool def = false)
        => r.HasColumn(col) && !r.IsDBNull(r.GetOrdinal(col))
            ? r.GetBoolean(r.GetOrdinal(col)) : def;
}
```

---

## 📡 API CLIENT SERVICE (MVC → API)

```csharp
public interface IApiClientService
{
    Task<ApiResponse<T>?> GetAsync<T>(string endpoint);
    Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object payload);
    Task<ApiResponse<T>?> PutAsync<T>(string endpoint, object payload);
    Task<ApiResponse<T>?> DeleteAsync<T>(string endpoint);
}

public class ApiClientService : IApiClientService
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiClientService> _logger;

    public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync(endpoint);
            return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET failed: {Endpoint}", endpoint);
            return ApiResponse<T>.Fail("API call failed.");
        }
    }

    // CRITICAL: Attach JWT from cookie to every API call
    private HttpClient CreateClient()
    {
        var client = _factory.CreateClient("MyProjectAPI");
        var token  = _httpContextAccessor.HttpContext?
            .Request.Cookies["AuthToken"];

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
```

---

## 📋 SESSION MANAGEMENT

### Rules
```
✅ Session stores: UserId, UserName, Email, RoleName, Permissions list
✅ Session timeout: 30 minutes (configurable in appsettings.json)
✅ JWT cookie: HttpOnly + Secure + SameSite=Strict
✅ On logout: clear session + expire cookie
✅ On session expired: redirect to /Login with returnUrl
✅ Sliding expiration: reset timer on each request
```

### Session Keys (constants)
```csharp
public static class SessionKeys
{
    public const string UserId      = "SESSION_USER_ID";
    public const string UserName    = "SESSION_USER_NAME";
    public const string Email       = "SESSION_EMAIL";
    public const string RoleName    = "SESSION_ROLE_NAME";
    public const string RoleId      = "SESSION_ROLE_ID";
    public const string Permissions = "SESSION_PERMISSIONS";
    public const string AuthToken   = "AuthToken";           // cookie name
}
```

---

## 🗄️ DATABASE TABLES — AUTH MODULE

```sql
-- Users table
MST_USER          (UserId, FullName, Email, PasswordHash, PasswordSalt,
                   RoleId, IsActive, IsDeleted, Audit cols)

-- Roles table
MST_ROLE          (RoleId, RoleName, Description,
                   IsActive, IsDeleted, Audit cols)

-- Modules table
MST_MODULE        (ModuleId, ModuleName, ModuleCode, DisplayOrder,
                   IsActive)

-- Role Permission matrix
MST_ROLE_PERMISSION (RolePermissionId, RoleId, ModuleId,
                     CanView, CanAdd, CanEdit, CanDelete, CanExport,
                     Audit cols)

-- Audit log
TRN_AUDIT_LOG     (LogId, UserId, Module, Action, Description,
                   IpAddress, UserAgent, LogDatetime)
```

---

## 🎨 FRONTEND STANDARDS

### Company Theme Colors (Extracted from Manpower Contract Login Screenshot)
```css
/* ═══ PRIMARY BRAND — Crimson Red ═══ */
--primary-color:          #db2128;   /* Brand red — left panel, accents, table headers */
--primary-dark:           #A31D28;   /* Darker red — hover states */
--primary-light:          #E8434F;   /* Lighter red — highlights */

/* ═══ CTA / ACTION — Teal Green ═══ */
--teal-color:             #009688;   /* Teal — Login btn, Save btn, links */
--teal-dark:              #00796B;   /* Dark teal — button hover */
--teal-light:             #4DB6AC;   /* Light teal — highlights */

/* ═══ ACCENT — Gold/Yellow (Title bar accent) ═══ */
--accent-gold:            #FFB300;   /* Gold accent bar next to title */
--accent-gold-light:      #FFD54F;   /* Light gold — badges */

/* ═══ LAYOUT ═══ */
--login-left-bg:          #db2128;   /* Solid crimson red left panel */
--sidebar-bg:             #1A1A2E;   /* Dark charcoal sidebar */
--sidebar-hover:          #2A2A42;   /* Sidebar item hover */
--sidebar-active:         #db2128;   /* Active item — brand red highlight */
--navbar-bg:              #FFFFFF;   /* White top navbar */
--navbar-border:          #E0E0E0;   /* Navbar bottom border */
--page-bg:                #F0F2F5;   /* Light gray page bg */
--card-bg:                #FFFFFF;   /* White cards */

/* ═══ TEXT ═══ */
--text-on-dark:           #FFFFFF;   /* White text on dark/red bg */
--text-on-dark-muted:     #E0E0E0;   /* Muted text on dark bg */
--text-primary:           #212529;   /* Body text */
--text-secondary:         #6C757D;   /* Muted text */
--text-heading:           #212529;   /* Headings — dark */
--text-brand-red:         #db2128;   /* Red branded text (footer tagline) */

/* ═══ TABLE ═══ */
--table-header-bg:        #db2128;   /* Crimson red header */
--table-header-text:      #FFFFFF;   /* White header text */
--table-row-hover:        #FFF3F4;   /* Light pink hover */
--table-border:           #DEE2E6;   /* Light gray borders */

/* ═══ MODAL ═══ */
--modal-header-bg:        #db2128;   /* Red modal header */
--modal-header-text:      #FFFFFF;   /* White modal title */

/* ═══ FORM ELEMENTS ═══ */
--input-border:           #CED4DA;   /* Default input border */
--input-focus-border:     #009688;   /* Teal focus border */
--input-focus-bg:         #E3F2FD;   /* Light blue focus background */
--input-focus-shadow:     rgba(0,150,136,0.25);  /* Teal glow */

/* ═══ BUTTONS ═══ */
--btn-primary-bg:         #009688;   /* Teal — Login, Save, Submit */
--btn-primary-text:       #FFFFFF;
--btn-primary-hover:      #00796B;   /* Dark teal hover */
--btn-secondary-bg:       #db2128;   /* Red — branded secondary */
--btn-secondary-text:     #FFFFFF;
--btn-outline-color:      #009688;   /* Teal outline buttons */
--btn-danger-bg:          #DC3545;   /* Red — delete */

/* ═══ STATUS ═══ */
--danger-color:         #DC3545;   /* Red — delete/error */
--warning-color:        #FFC107;   /* Yellow — warning */
--success-color:        #28A745;   /* Green — success */
--info-color:           #17A2B8;   /* Teal — info */

/* ═══ DECORATIVE ═══ */
--circle-decoration:      #F0F0F0;   /* Light gray decorative circles */
```
> ✅ Colors extracted from **Manpower Contract Management** login screenshot — Crimson Red + Teal theme

### Layout Rules
```
✅ Sticky sidebar with role-based menu items (hide if no permission)
✅ Top navbar: Logo + Username + Logout button
✅ Breadcrumb on every page
✅ All modals: Bootstrap 5 + data-bs-backdrop="static"
✅ All grids: DataTables.js + crimson red sticky header + col-search row
✅ All dropdowns: Select2 with dropdownParent fix for modals
✅ All toasts/alerts: SweetAlert2 only (never browser alert)
✅ Page loader: global spinner overlay
```

### JS Standards
```javascript
// ✅ ALWAYS — CSRF token on every POST
var csrfToken = $('input[name="__RequestVerificationToken"]').val();
headers: { 'RequestVerificationToken': csrfToken }

// ✅ ALWAYS — Show loader before AJAX, hide in success AND error
ShowLoader(true);
$.ajax({ ...,
    success: function(res) { ShowLoader(false); ... },
    error:   function()    { ShowLoader(false); ... }
});

// ✅ ALWAYS — Handle both success:false and HTTP errors
success: function(res) {
    ShowLoader(false);
    if (res.success) { ... }
    else { ShowToast(res.message, 'error'); }
},
error: function() {
    ShowLoader(false);
    ShowToast('An unexpected error occurred.', 'error');
}
```

---

## 📁 COMPLETE FOLDER STRUCTURE

```
MyProject.sln
├── src/
│   ├── MyProject.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── UserController.cs
│   │   │   ├── RoleController.cs
│   │   │   ├── RolePermissionController.cs
│   │   │   └── [FeatureController].cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionMiddleware.cs
│   │   │   └── RequestLoggingMiddleware.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   ├── LoginResponseDto.cs
│   │   │   │   └── ChangePasswordDto.cs
│   │   │   ├── User/
│   │   │   ├── Role/
│   │   │   └── [Feature]/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── MyProject.Application/
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── IBaseRepository.cs
│   │   │   │   ├── IAuthRepository.cs
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   ├── IRoleRepository.cs
│   │   │   │   └── I[Feature]Repository.cs
│   │   │   └── Services/
│   │   │       ├── IAuthService.cs
│   │   │       ├── IUserService.cs
│   │   │       ├── IRoleService.cs
│   │   │       └── I[Feature]Service.cs
│   │   └── Common/
│   │       ├── ApiResponse.cs
│   │       └── Constants.cs
│   │
│   ├── MyProject.Infrastructure/
│   │   ├── Repositories/
│   │   │   ├── BaseRepository.cs
│   │   │   ├── AuthRepository.cs
│   │   │   ├── UserRepository.cs
│   │   │   ├── RoleRepository.cs
│   │   │   └── [Feature]Repository.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── UserService.cs
│   │   │   ├── RoleService.cs
│   │   │   └── [Feature]Service.cs
│   │   ├── Helpers/
│   │   │   ├── PasswordHelper.cs
│   │   │   └── JwtHelper.cs
│   │   └── Extensions/
│   │       └── SqlDataReaderExtensions.cs
│   │
│   └── MyProject.MVC/
│       ├── Controllers/
│       │   ├── AccountController.cs   (Login/Logout)
│       │   ├── DashboardController.cs
│       │   ├── UserManagementController.cs
│       │   ├── RoleManagementController.cs
│       │   ├── RolePermissionController.cs
│       │   └── [Feature]Controller.cs
│       ├── Filters/
│       │   ├── AuthenticationFilter.cs
│       │   └── PermissionFilter.cs
│       ├── Services/
│       │   └── ApiClientService.cs
│       ├── ViewModels/
│       ├── Views/
│       │   ├── Account/
│       │   │   └── Login.cshtml
│       │   ├── Dashboard/
│       │   │   └── Index.cshtml
│       │   ├── UserManagement/
│       │   ├── RoleManagement/
│       │   ├── RolePermission/
│       │   ├── [Feature]/
│       │   ├── Shared/
│       │   │   ├── _Layout.cshtml
│       │   │   ├── _Sidebar.cshtml
│       │   │   ├── _Navbar.cshtml
│       │   │   └── _Footer.cshtml
│       │   └── Error/
│       │       ├── Unauthorized.cshtml
│       │       ├── NotFound.cshtml
│       │       └── ServerError.cshtml
│       ├── wwwroot/
│       │   ├── css/
│       │   │   └── theme.css
│       │   └── js/
│       │       ├── site.js
│       │       ├── account.js
│       │       ├── user-management.js
│       │       ├── role-management.js
│       │       ├── role-permission.js
│       │       └── [feature].js
│       ├── Program.cs
│       └── appsettings.json
│
├── Database/
│   ├── 01_CreateTables.sql
│   ├── 02_Auth_Tables.sql
│   ├── 03_StoredProcedures.sql
│   ├── 04_Auth_StoredProcedures.sql
│   └── 05_SeedData.sql
│
├── .gitlab-ci.yml
├── .gitignore
└── README.md
```

---

## 📦 NUGET PACKAGES REQUIRED

### API Project
```xml
<PackageReference Include="Microsoft.Data.SqlClient"     Version="5.2.2" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="ClosedXML"                    Version="0.102.3" />
<PackageReference Include="Swashbuckle.AspNetCore"       Version="6.5.0" />
<PackageReference Include="Serilog.AspNetCore"           Version="8.0.0" />
<PackageReference Include="BCrypt.Net-Next"              Version="4.0.3" />
```

### MVC Project
```xml
<PackageReference Include="Microsoft.AspNetCore.Session"  Version="2.3.0" />
<PackageReference Include="Newtonsoft.Json"               Version="13.0.3" />
```

---

## 🔑 APPSETTINGS STRUCTURE

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ManpowerDB;..."
  },
  "JwtSettings": {
    "Secret":         "YOUR_SUPER_SECRET_KEY_MIN_32_CHARS",
    "Issuer":         "MyProject.API",
    "Audience":       "MyProject.MVC",
    "ExpiryMinutes":  480
  },
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001"
  },
  "SessionSettings": {
    "TimeoutMinutes": 30,
    "CookieName":     "ManpowerSession"
  },
  "Logging": {
    "LogLevel": { "Default": "Information" }
  }
}
```

---

*skill.md | Manpower Contract Project | 2026*

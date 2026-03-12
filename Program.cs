using ManpowerContract.Repositories;
using ManpowerContract.Services;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ──────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options => {
        options.SerializerSettings.ContractResolver =
            new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    });

// ── DEPENDENCY INJECTION ─────────────────────────────────────────
builder.Services.AddScoped<IManpowerContractRepository, ManpowerContractRepository>();
builder.Services.AddScoped<IManpowerContractService,    ManpowerContractService>();

// ── ANTIFORGERY ──────────────────────────────────────────────────
builder.Services.AddAntiforgery(options => {
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ManpowerContract}/{action=Index}/{id?}");

app.Run();

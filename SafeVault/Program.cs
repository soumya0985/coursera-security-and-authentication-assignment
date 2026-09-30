using Microsoft.AspNetCore.Identity;
using SafeVault.Models;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Services;
using SafeVault.Services.Interfaces;
using SafeVault.Configuration;
using SafeVault.Helpers;
var builder = WebApplication.CreateBuilder(args);

#region Configure services
// Add services to the container.
var configuration = builder.Configuration;
var validationRulesSection =
    configuration.GetSection("ValidationRules");

builder.Services.Configure<ValidationOptions>(
    validationRulesSection);

var validationOptions =
    validationRulesSection.Get<ValidationOptions>()
    ?? new ValidationOptions();

// Configure Identity options based on validation rules from appsettings.json
// Add Password validation based on the configured validation rules
// Anti-forgery and other security-related configurations can be added here if needed.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters =
        validationOptions.AllowedUsernameCharacters
        ?? string.Empty;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
});

// Configure the application's database context using an in-memory database for simplicity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SafeVault"));

// We are configuring Identity with default settings and linking it to the application's database context
builder.Services
    .AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(Roles.Admin));
});

// Configure Razor Pages with authorization conventions
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Register");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");

    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

// Configure dependency injection for application services
builder.Services.AddScoped<IUserRegistrationService, RegistrationService>();
builder.Services.AddScoped<IUserAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IUserService, UserService>();

#endregion

#region Build and configure the application
var app = builder.Build();

// Configure application cookie settings
// Enforce login and access denied paths for the application cookie
// HTTP-only and secure cookie settings for the application cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";

    options.Cookie.HttpOnly = true;
    
    options.Cookie.SecurePolicy =
    app.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;

    options.SlidingExpiration = true;
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

#endregion

// Seed initial data for roles and users in the application
#region Seed initial data for roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    var roleManager = services
        .GetRequiredService<RoleManager<IdentityRole>>();

    // Seed roles
    await SeedDataHelper.SeedRolesAsync(roleManager);

    // Seed users
    await SeedDataHelper.SeedUsersAsync(userManager);
}
#endregion

await app.RunAsync();

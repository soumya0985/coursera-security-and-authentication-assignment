# SafeVault

SafeVault is a security-focused ASP.NET Core Razor Pages application developed to demonstrate secure authentication, authorization, input validation, and secure development practices using ASP.NET Core Identity and Entity Framework Core.

## Project Overview

The application implements:

- User registration
- User authentication (login/logout)
- Role-Based Access Control (RBAC)
- Input validation
- Password hashing through ASP.NET Core Identity
- Secure authentication cookies
- Account lockout protection
- Dependency Injection (DI)
- Unit testing using NUnit and Moq

The project uses:

- ASP.NET Core Razor Pages
- ASP.NET Core Identity
- Entity Framework Core
- Entity Framework Core In-Memory Provider
- NUnit
- Moq

---

# Use of Microsoft Copilot

Microsoft Copilot was used throughout the development lifecycle of the SafeVault project as an AI-powered development assistant.

Copilot was utilized to:

- Generate example code
- Refactor existing code
- Explain ASP.NET Core Identity concepts
- Review architectural decisions
- Recommend Dependency Injection patterns
- Create and improve unit tests
- Troubleshoot compilation and runtime issues
- Debug authentication and authorization problems
- Review security implementations
- Identify SQL Injection mitigation strategies
- Explain secure password handling and session management
- Assist with role-based access control design

All generated code, recommendations, and architectural suggestions were reviewed, validated, and adapted before being incorporated into the final solution.

---

# Examples of Copilot Assistance

## 1. Database Refactoring

### Original Approach

The project initially used direct SQLite access with manual SQL commands.

Example:

```csharp
using var connection = new SqliteConnection(connectionString);

using var command = connection.CreateCommand();
command.CommandText = queryCheckUserExists;
```

### Copilot Guidance

Copilot provided guidance on:

- Migrating from raw SQL access to Entity Framework Core
- Using IdentityDbContext
- Eliminating direct SQL queries where possible
- Leveraging LINQ for data access

Example:

```csharp
return await _context.Users.AnyAsync(u =>
    u.UserName == username &&
    u.Email == email);
```

Benefits:

- Reduced code complexity
- Improved maintainability
- Parameterized queries generated automatically by EF Core
- Reduced SQL Injection risk

---

## 2. ASP.NET Core Identity Integration

Copilot assisted in configuring ASP.NET Core Identity and replacing custom authentication implementations.

Example:

```csharp
builder.Services
    .AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

This enabled:

- User registration
- Authentication
- Password hashing
- Role management
- Cookie-based authentication

without implementing those features manually.

---

## 3. Password Security

The coursework required secure password handling and storage.

Copilot identified that ASP.NET Core Identity already provides:

- Secure password hashing
- Salt generation
- Password validation
- Password verification
- Hash algorithm upgrades

through:

```csharp
UserManager<IdentityUser>
```

and

```csharp
SignInManager<IdentityUser>
```

Example:

```csharp
await _userManager.CreateAsync(user, password);
```

Passwords are never stored or compared in plaintext.

---

## 4. Authentication and Login Functionality

Copilot provided guidance on secure login implementation.

Example:

```csharp
public async Task<bool> LoginAsync(
    string username,
    string password)
{
    var result =
        await _signInManager.PasswordSignInAsync(
            username,
            password,
            false,
            false);

    return result.Succeeded;
}
```

Copilot also explained:

- Authentication cookies
- Session cookies
- Persistent cookies
- Logout handling
- Login redirects
- Cookie security settings

---

## 5. Role-Based Access Control (RBAC)

Copilot assisted in implementing role-based authorization.

### Role Creation

```csharp
await roleManager.CreateAsync(
    new IdentityRole(Roles.Admin));
```

### Authorization Policy

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireRole(Roles.Admin));
});
```

### Razor Page Protection

```csharp
options.Conventions.AuthorizeFolder(
    "/Admin",
    "AdminOnly");
```

### Menu Visibility

```cshtml
@if (User.IsInRole(Roles.Admin))
{
    <a asp-page="/Admin/Dashboard">
        Admin Dashboard
    </a>
}
```

Copilot also provided guidance on:

- AccessDenied pages
- Authorization policies
- Role seeding
- Admin dashboard protection

---

## 6. Dependency Injection Refactoring

The project initially used static service classes.

Copilot assisted in refactoring the application to use Dependency Injection.

Example:

```csharp
public class RegistrationService :
    IUserRegistrationService
{
    private readonly UserManager<IdentityUser>
        _userManager;

    public RegistrationService(
        UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
}
```

Services were registered using:

```csharp
builder.Services.AddScoped<
    IUserRegistrationService,
    RegistrationService>();
```

Benefits:

- Improved testability
- Reduced coupling
- Better maintainability
- Alignment with ASP.NET Core best practices

---

## 7. Security Validation

Copilot provided guidance on implementing secure input validation.

Validation examples include:

```csharp
ValidationHelper.IsValidInput(
    username,
    allowedUsernameCharacters);
```

Additional recommendations included:

- Restricting allowed username characters
- Configuring Identity username restrictions
- Validating user input before processing
- Preventing invalid user registration attempts

Example:

```csharp
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters =
        validationOptions
            .AllowedUsernameCharacters;
});
```

---

## 8. SQL Injection Mitigation

Microsoft Copilot provided guidance regarding SQL Injection risks and mitigation strategies.

### Initial Approach

The project initially used parameterized SQLite queries:

```csharp
command.CommandText = queryCheckUserExists;

command.Parameters.AddWithValue(
    "@username",
    username);

command.Parameters.AddWithValue(
    "@email",
    email);
```

This reduced SQL Injection risks compared to string concatenation.

### Improved Approach

As the project evolved, Copilot recommended transitioning to:

- Entity Framework Core
- ASP.NET Core Identity
- LINQ-based queries

Examples:

```csharp
await _userManager.FindByNameAsync(
    username);
```

and:

```csharp
await _context.Users.AnyAsync(u =>
    u.UserName == username &&
    u.Email == email);
```

Entity Framework Core automatically produces parameterized database queries, significantly reducing SQL Injection risks.

### SQL Injection Testing

Copilot assisted in creating unit tests that validate the application's ability to reject malicious input.

Example:

```csharp
[Test]
public async Task
    CheckRegisteredUserAsync_RejectsSqlInjectionInput()
{
    var result =
        await service.CheckRegisteredUserAsync(
            "' OR 1=1 --",
            "user@example.com");

    Assert.That(result, Is.False);
}
```

---

## 9. Cross-Site Scripting (XSS) Protection

Copilot provided guidance on preventing XSS attacks.

Recommendations included:

- Input validation
- Identity username restrictions
- Razor HTML encoding
- Avoiding Html.Raw on user-generated content

Example test:

```csharp
[Test]
public async Task
    CheckRegisteredUserAsync_RejectsXssInput()
{
    var result =
        await service.CheckRegisteredUserAsync(
            "<script>alert('xss')</script>",
            "user@example.com");

    Assert.That(result, Is.False);
}
```

Copilot also explained Razor's automatic output encoding and the importance of safely rendering user-controlled data.

---

## 10. Authentication Cookie Security

Copilot assisted in configuring secure authentication cookies.

Example:

```csharp
builder.Services.ConfigureApplicationCookie(
    options =>
{
    options.Cookie.HttpOnly = true;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    options.SlidingExpiration = true;
});
```

Benefits:

- Reduced client-side script access
- Secure cookie transmission
- Improved authentication security

---

## 11. Identity Lockout Protection

Copilot recommended implementing account lockout protections.

Example:

```csharp
options.Lockout.MaxFailedAccessAttempts = 5;

options.Lockout.DefaultLockoutTimeSpan =
    TimeSpan.FromMinutes(15);

options.Lockout.AllowedForNewUsers = true;
```

This helps mitigate:

- Password guessing attacks
- Automated brute-force attempts

---

## 12. Unit Testing

Copilot assisted in creating and improving unit tests using:

- NUnit
- Moq
- ASP.NET Core Identity mocks

Example:

```csharp
var mockUserManager =
    CreateMockUserManager();
```

and:

```csharp
var registrationService =
    new RegistrationService(
        mockUserManager.Object);
```

Copilot also explained:

- Mocking UserManager
- Mocking SignInManager
- Dependency Injection testing
- Service-level testing strategies

Example:

```csharp
mockUserManager.Verify(
    x => x.FindByNameAsync(
        It.IsAny<string>()),
    Times.Never);
```

This validates that malicious inputs are rejected before Identity interactions occur.

---

## 13. Debugging and Troubleshooting Assistance

Copilot was regularly used as a debugging and troubleshooting companion throughout development.

### Authentication Troubleshooting

Copilot assisted with:

- Login form submission issues
- Authentication cookie configuration
- SignInManager setup
- Redirect loops
- Authorization failures

Examples included diagnosing:

```csharp
OnPostAsync()
```

not being reached during login attempts and identifying form configuration issues such as:

```html
<form method="post">
```

### Dependency Injection Issues

Copilot helped resolve issues including:

```text
CS0246
The type or namespace name
'IRegistrationService'
could not be found
```

by identifying:

- Missing namespaces
- Incorrect interfaces
- Service registration issues
- Dependency injection configuration errors

### Identity Configuration Issues

Copilot provided guidance on:

- UserManager setup
- RoleManager registration
- SignInManager usage
- Identity options
- Role-based authorization

### Unit Test Debugging

Copilot helped troubleshoot:

- Mocking problems
- Dependency injection testing
- Identity mock implementations
- Service instantiation errors

### Architecture Reviews

Throughout the project, Copilot was also used to evaluate architectural decisions such as:

- Static classes versus Dependency Injection
- Authentication service design
- Authorization service design
- Configuration management
- Service separation and responsibilities

---

## 14. Project Structure Improvements

Copilot assisted in organizing the application into a clean and maintainable architecture:

```text
Configuration/
Data/
Helpers/
Models/
Pages/
Services/
Services/Interfaces/
Tests/
```

Additional improvements included:

- ValidationOptions
- Role constants
- Identity seeding
- Authentication services
- Authorization services
- Registration services

---

# Security Features Implemented

The final SafeVault application includes:

- ASP.NET Core Identity authentication
- Cookie-based authentication
- Secure password hashing
- Role-Based Access Control (RBAC)
- Authorization policies
- Input validation
- Username restrictions
- SQL Injection mitigation through EF Core parameterization
- XSS mitigation through validation and Razor encoding
- Secure authentication cookies
- Account lockout protection
- Dependency Injection architecture
- Unit testing with NUnit and Moq

---

# Disclaimer

Microsoft Copilot was used throughout the SafeVault project as a software development assistant. All suggestions, code samples, architectural recommendations, debugging guidance, and security practices generated by Copilot were reviewed, tested, and validated before inclusion in the final implementation.

SafeVault's final design decisions, code integration, testing, and security verification were performed by the project developer.

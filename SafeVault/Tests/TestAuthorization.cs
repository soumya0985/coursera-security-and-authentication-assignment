using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using SafeVault.Pages.Admin;
using SafeVault.Services;
using SafeVault.Services.Interfaces;

namespace SafeVault.Tests;

[TestFixture]
public class TestAuthorization
{
    private Mock<UserManager<IdentityUser>> _mockUserManager = null!;
    private IUserAuthorizationService _authorizationService = null!;
    private Mock<UserService> _mockUserService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            new Mock<IUserStore<IdentityUser>>().Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        _mockUserService = new Mock<UserService>(_mockUserManager.Object);

        _authorizationService = new AuthorizationService(
            _mockUserManager.Object);
    }

    [Test]
	public void DashboardModel_ShouldRequireAdminRole()
	{
		var authorizeAttribute = typeof(DashboardModel)
			.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
			.Cast<AuthorizeAttribute>()
			.FirstOrDefault();

		Assert.That(authorizeAttribute, Is.Not.Null);
		Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
	}

	[Test]
	public async Task OnGet_ShouldLogUserName_WhenAccessed()
	{
		var loggerMock = new Mock<ILogger<DashboardModel>>();
		var model = new DashboardModel(loggerMock.Object, _mockUserService.Object)
		{
			PageContext = new()
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(
						new ClaimsIdentity(new[]
						{
							new Claim(ClaimTypes.Name, "alice")
						}, "TestAuthType"))
				}
			}
		};

		await model.OnGet();

		loggerMock.Verify(
			x => x.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("alice", StringComparison.Ordinal)),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

    /// <summary>
    /// Included this because typically we can't test middleware directly in unit tests.
    /// Tests that a regular user cannot manage users.
    /// This ensures that only users with the "admin" role have the ability to manage users.
    /// This is a negative test case for the authorization service.    /// 
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task UserCanManageUsersAsync_ReturnsFalse_ForRegularUser()
    {
        var user = new IdentityUser();

        _mockUserManager
            .Setup(x => x.IsInRoleAsync(user, "admin"))
            .ReturnsAsync(false);

        var result = await _authorizationService
            .UserCanManageUsersAsync(user);

        Assert.That(result, Is.False);
    }
}

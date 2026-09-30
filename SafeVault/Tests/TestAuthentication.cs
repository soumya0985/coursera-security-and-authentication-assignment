using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using SafeVault.Configuration;
using SafeVault.Services;
using NUnit.Framework;

namespace SafeVault.Tests;

[TestFixture]
public class TestAuthentication
{
	[Test]
	public async Task LoginAsync_ReturnsTrue_WhenCredentialsAreValid_AndSignInSucceeds()
	{
		// Arrange
		var signInManagerMock = CreateMockSignInManager();
		signInManagerMock
			.Setup(s => s.PasswordSignInAsync("validUser", "validPassword", false, false))
			.ReturnsAsync(SignInResult.Success);

		var options = Options.Create(new ValidationOptions
		{
			AllowedUsernameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
		});

        var loggerMock = new Mock<ILogger<AuthenticationService>>(); 

		var service = new AuthenticationService(signInManagerMock.Object, options, loggerMock.Object);

		// Act
		var result = await service.LoginAsync("validUser", "validPassword");

		// Assert
		Assert.IsTrue(result);
		signInManagerMock.Verify(s => s.PasswordSignInAsync("validUser", "validPassword", false, false), Times.Once);
	}

	[Test]
	public async Task LoginAsync_ReturnsFalse_WhenUsernameIsInvalid()
	{
		// Arrange
		var signInManagerMock = CreateMockSignInManager();

		var options = Options.Create(new ValidationOptions
		{
			AllowedUsernameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
		});

		var loggerMock = new Mock<ILogger<AuthenticationService>>(); 
		var service = new AuthenticationService(signInManagerMock.Object, options, loggerMock.Object);

		// Act
		var result = await service.LoginAsync("invalid$user", "anyPassword");

		// Assert
		Assert.IsFalse(result);
		signInManagerMock.Verify(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
	}

	private static Mock<SignInManager<IdentityUser>> CreateMockSignInManager()
	{
		var userManagerMock = new Mock<UserManager<IdentityUser>>(
			new Mock<IUserStore<IdentityUser>>().Object,
			null!,
			null!,
			null!,
			null!,
			null!,
			null!,
			null!,
			null!);

		return new Mock<SignInManager<IdentityUser>>(
			userManagerMock.Object,
			new Mock<IHttpContextAccessor>().Object,
			new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
			null!,
			null!,
			null!,
			null!);
	}
}

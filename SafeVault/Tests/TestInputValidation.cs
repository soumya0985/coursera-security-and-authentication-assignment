using NUnit.Framework;
using SafeVault.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.Extensions.Options;
using SafeVault.Configuration;
using Microsoft.AspNetCore.Authentication;
using SafeVault.Services.Interfaces;

namespace SafeVault.Tests
{
    [TestFixture]
    public class TestInputValidation
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager = null!;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager = null!;
        private Mock<SignInManager<IdentityUser>> _mockSignInManager = null!;
        private IUserRegistrationService _registrationService = null!;
        private IUserAuthenticationService _authenticationService = null!;
        private Mock<ILogger<Services.AuthenticationService>> _mockAuthenticationLogger = null!;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = CreateMockUserManager();
            _mockRoleManager = CreateMockRoleManager();
            _mockSignInManager = CreateMockSignInManager();
            _registrationService = new RegistrationService(_mockUserManager.Object, _mockRoleManager.Object, Options.Create(new ValidationOptions()));
            _mockAuthenticationLogger = new Mock<ILogger<Services.AuthenticationService>>();
            _authenticationService = new Services.AuthenticationService(_mockSignInManager.Object, Options.Create(new ValidationOptions()), _mockAuthenticationLogger.Object);
        }

        private static Mock<UserManager<IdentityUser>> CreateMockUserManager()
        {
            //Minimal setup for UserManager mocking
            var store = new Mock<IUserStore<IdentityUser>>();

            return new Mock<UserManager<IdentityUser>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        private static Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();

            return new Mock<RoleManager<IdentityRole>>(
                store.Object,
                null!,
                null!,
                null!,
                null!);
        }

        private static Mock<SignInManager<IdentityUser>> CreateMockSignInManager()
        {
            var userManager = CreateMockUserManager();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<IdentityUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<IdentityUser>>();

            return new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object);
        }

        /// <summary>
        /// Tests that SQL injection input is rejected by the CheckRegisteredUserAsync method.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CheckRegisteredUserAsync_RejectsSqlInjectionInput()
        {
            var result = await _registrationService.CheckRegisteredUserAsync(
                "' OR 1=1 --",
                "user@example.com");

            Assert.That(result, Is.False);

            _mockUserManager.Verify(
                x => x.FindByNameAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        /// <summary>
        /// Tests that XSS input is rejected by the CheckRegisteredUserAsync method.
        /// </summary>
        /// <returns></returns>
        public async Task CheckRegisteredUserAsync_RejectsXssInput()
        {
            var xssUsername = "<script>alert('xss')</script>";
            var validEmail = "user@example.com";
            
            var result = await _registrationService.CheckRegisteredUserAsync(
                xssUsername,
                validEmail);

            Assert.That(result, Is.False);

            _mockUserManager.Verify(
                x => x.FindByNameAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public async Task RegisterUserAsync_RejectsSqlInjectionInput()
        {
            var result = await _registrationService.RegisterUserAsync(
                "' OR 1=1 --",
                "user@example.com",
                "Password123!",
                "User");

            Assert.That(result.Succeeded, Is.False);

            _mockRoleManager.Verify(
                x => x.RoleExistsAsync(It.IsAny<string>()),
                Times.Never);
            _mockUserManager.Verify(
                x => x.FindByNameAsync(It.IsAny<string>()),
                Times.Never);
            _mockUserManager.Verify(
                x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public async Task RegisterUserAsync_RejectsInvalidEmailFormat()
        {
            var result = await _registrationService.RegisterUserAsync(
                "validuser",
                "bad email",
                "Password123!",
                "User");

            Assert.That(result.Succeeded, Is.False);

            _mockRoleManager.Verify(
                x => x.RoleExistsAsync(It.IsAny<string>()),
                Times.Never);
            _mockUserManager.Verify(
                x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public async Task LoginAsync_RejectsSqlInjectionUsername()
        {
            var result = await _authenticationService.LoginAsync(
                "' OR 1=1 --",
                "Password123!");

            Assert.That(result, Is.False);

            _mockSignInManager.Verify(
                x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Test]
        public async Task LoginAsync_RejectsEmptyPassword()
        {
            var result = await _authenticationService.LoginAsync(
                "validuser",
                string.Empty);

            Assert.That(result, Is.False);

            _mockSignInManager.Verify(
                x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Never);
        }
    }
}
using Moq;
using SOCApi.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Data.SqlClient;
using System.Threading;

namespace SOCApi.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IUrlHelperFactory> _urlHelperFactoryMock; // Added mock for IUrlHelperFactory
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("your-secure-jwt-key-here");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("your-issuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("your-audience");

            _urlHelperFactoryMock = new Mock<IUrlHelperFactory>(); // Initialize the mock

            _controller = new AuthController(_configurationMock.Object, _urlHelperFactoryMock.Object); // Pass the mock to the constructor

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/redirect-url");
            _controller.Url = urlHelperMock.Object; // Assign the mocked IUrlHelper to the controller
        }

        [Fact]
        public async Task Login_ReturnsChallengeResult()
        {
            // Arrange
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = "/",
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, "test_user"),
                    new(ClaimTypes.Name, "test_user")
                };
            var claimsIdentity = new ClaimsIdentity(claims, GoogleDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Ensure HttpContext is initialized
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(new Mock<IAuthenticationService>().Object);
            _controller.ControllerContext.HttpContext.RequestServices = serviceProviderMock.Object;
            _controller.ControllerContext.HttpContext.User = claimsPrincipal;

            // Act
            var result = await _controller.GoogleLogin();

            // Assert
            var challengeResult = Assert.IsType<ChallengeResult>(result);
            Assert.Contains(GoogleDefaults.AuthenticationScheme, challengeResult.AuthenticationSchemes);
        }

        /// <summary>
        /// Callback endpoint for Google authentication.
        /// This method is called after the user has authenticated with Google.
        /// </summary>
        /// <returns>
        /// Returns a JWT token if authentication is successful.
        /// </returns>
        [Fact]
        public async Task Callback_AuthenticationSucceeds_ReturnsJwtToken()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "test_user"),
                new(ClaimTypes.Email, "test@example.com")
            };

            var claimsIdentity = new ClaimsIdentity(claims, GoogleDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, GoogleDefaults.AuthenticationScheme));
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
            .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), GoogleDefaults.AuthenticationScheme))
            .ReturnsAsync(authResult);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/redirect-url");
            _controller.Url = urlHelperMock.Object; // Assign the mocked IUrlHelper to the controller

            // Act
            var result = await _controller.GoogleCallback();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var token = Assert.IsType<string>(okResult.Value);
            Assert.NotNull(token);

            // Decode the JWT token to verify its contents
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal("test_user", jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal("test@example.com", jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        }

        [Fact]
        public async Task Callback_AuthenticationFails_ReturnsBadRequest()
        {
            // Arrange
            var authResult = AuthenticateResult.Fail("Authentication failed");
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), GoogleDefaults.AuthenticationScheme))
                .ReturnsAsync(authResult);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            // Act
            var result = await _controller.GoogleCallback();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GoogleLogout_RedirectsToGoogleLogin()
        {
            // Arrange
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = "/google-login"
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.SignOutAsync(GoogleDefaults.AuthenticationScheme, It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            httpContextMock.Setup(c => c.SignOutAsync()).Returns(Task.CompletedTask);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = await _controller.GoogleLogout();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/google-login", redirectResult.Url);
        }

        [Fact]
        public async Task RegisterOrGetUser_ValidInput_ReturnsUserId()
        {
            // Arrange
            var email = "test@example.com";
            var name = "Test User";
            var userId = 123;

            var sqlConnectionMock = new Mock<Microsoft.Data.SqlClient.SqlConnection>();
            var sqlCommandMock = new Mock<Microsoft.Data.SqlClient.SqlCommand>();

            sqlCommandMock.Setup(cmd => cmd.ExecuteScalarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            sqlConnectionMock.Setup(conn => conn.CreateCommand())
                .Returns(sqlCommandMock.Object);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(Microsoft.Data.SqlClient.SqlConnection)))
                .Returns(sqlConnectionMock.Object);

            _controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };

            // Act
            var result = await _controller.RegisterOrGetUser(email, name);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userId, okResult.Value);
        }
    }
}

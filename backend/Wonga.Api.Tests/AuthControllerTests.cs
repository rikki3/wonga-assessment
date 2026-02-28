using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wonga.Api.Controllers;
using Wonga.Api.Data;
using Wonga.Api.Services;

namespace Wonga.Api.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ValidRequest_CreatesUserAndReturnsToken()
    {
        // Arrange
        await using var db = CreateDbContext();
        var controller = new AuthController(db, CreateJwtService());

        var req = new AuthController.RegisterRequest(
            Email: "rickesh@roguegamestudio.space",
            Password: "roguegamestudio",
            FirstName: "Rickesh",
            LastName: "Singh");

        // Act
        var result = await controller.Register(req);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AuthController.AuthResponse>(ok.Value);

        Assert.False(string.IsNullOrWhiteSpace(body.Token));
        Assert.Equal("rickesh@roguegamestudio.space", body.Email);

        var user = await db.Users.SingleAsync(u => u.Email == "rickesh@roguegamestudio.space");
        Assert.Equal("Rickesh", user.FirstName);
        Assert.Equal("Singh", user.LastName);
        Assert.NotEqual(req.Password, user.PasswordHash);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        await using var db = CreateDbContext();
        var controller = new AuthController(db, CreateJwtService());

        var req = new AuthController.RegisterRequest(
            Email: "rickesh@roguegamestudio.space",
            Password: "roguegamestudio",
            FirstName: "Rickesh",
            LastName: "Singh");

        _ = await controller.Register(req);
        var result = await controller.Register(req);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Register_MissingNames_ReturnsBadRequest()
    {
        await using var db = CreateDbContext();
        var controller = new AuthController(db, CreateJwtService());

        var req = new AuthController.RegisterRequest(
            Email: "rickesh@roguegamestudio.space",
            Password: "roguegamestudio",
            FirstName: "Rickesh",
            LastName: "");

        var result = await controller.Register(req);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        await using var db = CreateDbContext();
        var controller = new AuthController(db, CreateJwtService());

        var registerReq = new AuthController.RegisterRequest(
            Email: "rickesh@roguegamestudio.space",
            Password: "roguegamestudio",
            FirstName: "Rickesh",
            LastName: "Singh");

        _ = await controller.Register(registerReq);

        var loginReq = new AuthController.LoginRequest(
            Email: "rickesh@roguegamestudio.space",
            Password: "roguegamestudio");

        var result = await controller.Login(loginReq);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AuthController.AuthResponse>(ok.Value);

        Assert.False(string.IsNullOrWhiteSpace(body.Token));
        Assert.Equal("rickesh@roguegamestudio.space", body.Email);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        await using var db = CreateDbContext();
        var controller = new AuthController(db, CreateJwtService());

        var registerReq = new AuthController.RegisterRequest(
            Email: "rickesh@roguegamestudio.space",
            Password: "roguegamestudio",
            FirstName: "Rickesh",
            LastName: "Singh");

        _ = await controller.Register(registerReq);

        var loginReq = new AuthController.LoginRequest(
            Email: "rickesh@roguegamestudio.space",
            Password: "studioroguegame");

        var result = await controller.Login(loginReq);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    private static WongaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<WongaDbContext>()
            .UseInMemoryDatabase($"wonga-auth-tests-{Guid.NewGuid()}")
            .Options;

        return new WongaDbContext(options);
    }

    private static JwtService CreateJwtService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "wonga",
                ["Jwt:Audience"] = "wonga",
                ["Jwt:Key"] = "00000000000000000000000000000000"
            })
            .Build();

        return new JwtService(config);
    }
}

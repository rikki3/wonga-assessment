using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wonga.Api.Controllers;
using Wonga.Api.Data;
using Wonga.Api.Models;

namespace Wonga.Api.Tests;

public class UserControllerTests
{
    [Fact]
    public async Task Info_WithValidIdentityClaim_ReturnsUserInfo()
    {
        await using var db = CreateDbContext();
        var user = new User
        {
            UserID = 101,
            Email = "rickesh@roguegamestudio.space",
            FirstName = "Rickesh",
            LastName = "Singh",
            PasswordHash = "roguegamestudio"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var controller = new UserController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithClaims(new Claim(ClaimTypes.NameIdentifier, "101"))
        };

        var result = await controller.Info();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<UserController.UserDetailsResponse>(ok.Value);
        Assert.Equal("Rickesh", body.FirstName);
        Assert.Equal("Singh", body.LastName);
        Assert.Equal("rickesh@roguegamestudio.space", body.Email);
    }

    [Fact]
    public async Task Info_WhenUserMissing_ReturnsNotFound()
    {
        await using var db = CreateDbContext();

        var controller = new UserController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = BuildHttpContextWithClaims(new Claim(ClaimTypes.NameIdentifier, "999"))
        };

        var result = await controller.Info();

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    private static WongaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<WongaDbContext>()
            .UseInMemoryDatabase($"wonga-user-tests-{Guid.NewGuid()}")
            .Options;

        return new WongaDbContext(options);
    }

    private static HttpContext BuildHttpContextWithClaims(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        return new DefaultHttpContext
        {
            User = principal
        };
    }
}

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wonga.Api.Data;

namespace Wonga.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController(WongaDbContext db) : ControllerBase
{
    public record UserDetailsResponse(string FirstName, string LastName, string Email);

    [Authorize]
    [HttpGet("info")]
    public async Task<ActionResult<UserDetailsResponse>> Info()
    {
        Wonga.Api.Models.User? user = null;

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdClaim, out var userId))
        {
            user = await db.Users.SingleOrDefaultAsync(u => u.UserID == userId);
        }

        if (user is null)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);

            if (!string.IsNullOrWhiteSpace(email))
            {
                var normalizedEmail = email.Trim().ToLowerInvariant();
                user = await db.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail);
            }
        }

        if (user is null) return NotFound("User not found.");

        return Ok(new UserDetailsResponse(user.FirstName, user.LastName, user.Email));
    }
}

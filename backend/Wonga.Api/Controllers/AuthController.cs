using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wonga.Api.Data;
using Wonga.Api.Models;
using Wonga.Api.Services;

namespace Wonga.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(WongaDbContext db, JwtService jwt) : ControllerBase
{
    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string Token, string Email);

    [HttpPost("register")]
    
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
    {
        var email = req.Email?.Trim().ToLowerInvariant();
        var firstName = req.FirstName?.Trim();
        var lastName = req.LastName?.Trim();

        if (string.IsNullOrWhiteSpace(firstName)
            || string.IsNullOrWhiteSpace(lastName)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(req.Password))
        {
            return BadRequest("First name, last name, email, and password are required.");
        }
        
        var exists = await db.Users.AnyAsync(u => u.Email == email);
        if (exists) return Conflict("Email already registered.");

        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(req.Password),
            FirstName = firstName,
            LastName = lastName
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = jwt.CreateToken(user);
        return Ok(new AuthResponse(token, user.Email));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var email = req.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Email and password are required.");

        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null) return Unauthorized("Invalid credentials.");

        if (!VerifyPassword(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = jwt.CreateToken(user);
        return Ok(new AuthResponse(token, user.Email));
    }

    private static string HashPassword(string password)
    {
        // PBKDF2 with random salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        // store: {salt}.{hash} base64
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split('.', 2);
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var actualHash = pbkdf2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}

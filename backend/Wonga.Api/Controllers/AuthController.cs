using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
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
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; init; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; init; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string LastName { get; init; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; init; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; init; } = string.Empty;
    }

    public record AuthResponse(string Token, string Email);

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
    {
        var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
        var firstName = (req.FirstName ?? string.Empty).Trim();
        var lastName = (req.LastName ?? string.Empty).Trim();
        var password = req.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName)
            || string.IsNullOrWhiteSpace(lastName)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(firstName))
                validationErrors["firstName"] = ["First name is required."];
            if (string.IsNullOrWhiteSpace(lastName))
                validationErrors["lastName"] = ["Last name is required."];
            if (string.IsNullOrWhiteSpace(email))
                validationErrors["email"] = ["Email is required."];
            if (string.IsNullOrWhiteSpace(password))
                validationErrors["password"] = ["Password is required."];

            return BadRequest(ApiErrorResponse.Validation(validationErrors));
        }
        
        var exists = await db.Users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            return Conflict(ApiErrorResponse.Create("email_exists", "Email already registered."));
        }

        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(password),
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
        var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
        var password = req.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(email))
                validationErrors["email"] = ["Email is required."];
            if (string.IsNullOrWhiteSpace(password))
                validationErrors["password"] = ["Password is required."];

            return BadRequest(ApiErrorResponse.Validation(validationErrors));
        }

        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return Unauthorized(ApiErrorResponse.Create("invalid_credentials", "Invalid credentials."));

        if (!VerifyPassword(password, user.PasswordHash))
            return Unauthorized(ApiErrorResponse.Create("invalid_credentials", "Invalid credentials."));

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

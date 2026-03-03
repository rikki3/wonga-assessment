using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wonga.Api.Data;
using Wonga.Api.Models;
using Wonga.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Config
var conn = builder.Configuration.GetConnectionString("Default") 
           ?? builder.Configuration["ConnectionStrings:Default"]
           ?? throw new InvalidOperationException("Connection string not configured.");

builder.Services.AddDbContext<WongaDbContext>(opt => opt.UseNpgsql(conn));
builder.Services.AddSingleton<JwtService>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(kvp => kvp.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    kvp => Program.ToCamelCaseField(kvp.Key),
                    kvp => kvp.Value!.Errors
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage)
                        .ToArray());

            return new BadRequestObjectResult(ApiErrorResponse.Validation(errors));
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray()
    ?? ["http://localhost:3000", "http://localhost:5173"];

// CORS restricted to known web origins
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowWeb", p =>
        p.WithOrigins(allowedOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

// JWT auth
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing.");
var issuer = builder.Configuration["Jwt:Issuer"] ?? "Wonga";
var audience = builder.Configuration["Jwt:Audience"] ?? "Wonga";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations automatically (simple for assessment)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WongaDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
    public static string ToCamelCaseField(string rawKey)
    {
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            return "request";
        }

        var key = rawKey.Replace("$.", string.Empty);
        var lastSegment = key.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "request";

        if (lastSegment.Length == 1)
        {
            return lastSegment.ToLowerInvariant();
        }

        return char.ToLowerInvariant(lastSegment[0]) + lastSegment[1..];
    }
}

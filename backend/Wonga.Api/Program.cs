using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wonga.Api.Data;
using Wonga.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Config
var conn = builder.Configuration.GetConnectionString("Default") 
           ?? builder.Configuration["ConnectionStrings:Default"]
           ?? throw new InvalidOperationException("Connection string not configured.");

builder.Services.AddDbContext<WongaDbContext>(opt => opt.UseNpgsql(conn));
builder.Services.AddSingleton<JwtService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for local/dev
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowWeb", p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true)); // ok for assessment/dev
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

var app = builder.Build();

// Apply migrations automatically (simple for assessment)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WongaDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
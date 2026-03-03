using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wonga.Api.Data;

namespace Wonga.Api.Tests;

public sealed class WongaApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"wonga-integration-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<WongaDbContext>>();
            services.AddDbContext<WongaDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}

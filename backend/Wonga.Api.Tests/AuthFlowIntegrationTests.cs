using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Wonga.Api.Tests;

public class AuthFlowIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Register_Login_ThenGetUserInfo_ReturnsExpectedUser()
    {
        await using var factory = new WongaApiFactory();
        using var client = factory.CreateClient();

        var email = $"integration-{Guid.NewGuid():N}@example.com";
        const string password = "IntegrationPass123!";
        const string firstName = "Integration";
        const string lastName = "Tester";

        var registerResponse = await client.PostAsJsonAsync("/auth/register", new
        {
            email,
            password,
            firstName,
            lastName
        });
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/auth/login", new
        {
            email,
            password
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(loginBody);
        Assert.False(string.IsNullOrWhiteSpace(loginBody!.Token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginBody.Token);

        var userInfoResponse = await client.GetAsync("/user/info");
        Assert.Equal(HttpStatusCode.OK, userInfoResponse.StatusCode);

        var userInfoBody = await userInfoResponse.Content.ReadFromJsonAsync<UserInfoResponse>(JsonOptions);
        Assert.NotNull(userInfoBody);
        Assert.Equal(firstName, userInfoBody!.FirstName);
        Assert.Equal(lastName, userInfoBody.LastName);
        Assert.Equal(email, userInfoBody.Email);
    }

    [Fact]
    public async Task GetUserInfo_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = new WongaApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/user/info");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private sealed record AuthResponse(string Token, string Email);
    private sealed record UserInfoResponse(string FirstName, string LastName, string Email);
}

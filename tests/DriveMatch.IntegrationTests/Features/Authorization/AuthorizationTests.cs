using System.Net;
using DriveMatch.IntegrationTests.Infrastructure;

namespace DriveMatch.IntegrationTests.Features.Authorization;

[Collection(IntegrationTestCollection.Name)]
public sealed class AuthorizationTests
{
    private readonly HttpClient _client;

    public AuthorizationTests(
        DriveMatchApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StudentProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync(
            "/api/students/profile");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}
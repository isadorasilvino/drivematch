using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DriveMatch.Application.Features.Auth.Login;
using DriveMatch.Application.Features.Users.Register;
using DriveMatch.Domain.Enums;
using DriveMatch.IntegrationTests.Infrastructure;

namespace DriveMatch.IntegrationTests.Features.Auth;

[Collection(IntegrationTestCollection.Name)]
public sealed class RegistrationAndLoginTests
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

    private readonly HttpClient _client;

    public RegistrationAndLoginTests(
        DriveMatchApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterStudent_ThenLogin_ShouldReturnAuthenticatedUser()
    {
        // Arrange
        var email =
            $"student-{Guid.NewGuid():N}@drivematch.test";

        var password = "DriveMatch@123";

        var registerRequest = new
        {
            Name = "Integration Student",
            Email = email,
            Password = password,
            Role = UserRole.Student
        };

        // Act - register
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/users/",
            registerRequest);

        // Assert - register
        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var registeredUser =
            await registerResponse.Content
                .ReadFromJsonAsync<RegisterUserResult>();

        Assert.NotNull(registeredUser);
        Assert.NotEqual(Guid.Empty, registeredUser.UserId);
        Assert.Equal("Integration Student", registeredUser.Name);
        Assert.Equal(email, registeredUser.Email);

        // Act - login
        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest);

        // Assert - login
        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResult>(IntegrationJson.Options);

        Assert.NotNull(loginResult);

        Assert.Equal(
            registeredUser.UserId,
            loginResult.UserId);

        Assert.Equal(
            registeredUser.Name,
            loginResult.Name);

        Assert.Equal(
            registeredUser.Email,
            loginResult.Email);

        Assert.Equal(
            UserRole.Student,
            loginResult.Role);

        Assert.False(
            string.IsNullOrWhiteSpace(loginResult.Token));
    }

    [Fact]
    public async Task LoginWithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var email =
            $"student-{Guid.NewGuid():N}@drivematch.test";

        var registerRequest = new
        {
            Name = "Integration Student",
            Email = email,
            Password = "DriveMatch@123",
            Role = UserRole.Student
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/users/",
            registerRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginRequest = new
        {
            Email = email,
            Password = "WrongPassword@123"
        };

        // Act
        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            loginResponse.StatusCode);
    }

    [Fact]
    public async Task RegisterWithExistingEmail_ShouldReturnConflict()
    {
        // Arrange
        var email =
            $"student-{Guid.NewGuid():N}@drivematch.test";

        var firstRequest = new
        {
            Name = "Integration Student",
            Email = email,
            Password = "DriveMatch@123",
            Role = UserRole.Student
        };

        var duplicateRequest = new
        {
            Name = "Another Student",
            Email = email,
            Password = "AnotherPassword@123",
            Role = UserRole.Student
        };

        var firstResponse = await _client.PostAsJsonAsync(
            "/api/users/",
            firstRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        // Act
        var duplicateResponse = await _client.PostAsJsonAsync(
            "/api/users/",
            duplicateRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.Conflict,
            duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task InstructorAccessingStudentEndpoint_ShouldReturnForbidden()
    {
        // Arrange
        var email =
            $"instructor-{Guid.NewGuid():N}@drivematch.test";

        var password = "DriveMatch@123";

        var registerRequest = new
        {
            Name = "Integration Instructor",
            Email = email,
            Password = password,
            Role = UserRole.Instructor
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/users/",
            registerRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResult>(JsonOptions);

        Assert.NotNull(loginResult);
        Assert.False(
            string.IsNullOrWhiteSpace(loginResult.Token));

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        // Act
        var response = await _client.GetAsync(
            "/api/students/profile");

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task StudentAccessingInstructorEndpoint_ShouldReturnForbidden()
    {
        // Arrange
        var email =
            $"student-role-{Guid.NewGuid():N}@drivematch.test";

        var password = "DriveMatch@123";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "Integration Student Role",
                Email = email,
                Password = password,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = password
            });

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResult>(
                    IntegrationJson.Options);

        Assert.NotNull(loginResult);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        // Act
        var response = await _client.GetAsync(
            "/api/instructors/profile");

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpointWithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync(
            "/api/students/profile");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task StudentProfile_CreateGetUpdate_ShouldPersistChanges()
    {
        // Arrange
        var email =
            $"student-profile-{Guid.NewGuid():N}@drivematch.test";

        var password = "DriveMatch@123";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "Integration Profile Student",
                Email = email,
                Password = password,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = password
            });

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResult>(
                    IntegrationJson.Options);

        Assert.NotNull(loginResult);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        var createRequest = new
        {
            City = "Belo Horizonte",
            State = "MG",
            ExperienceLevel = ExperienceLevel.Beginner,
            OwnsVehicle = false,
            HasOwnVehicleForLessons = false
        };

        // Act - create
        var createResponse = await _client.PostAsJsonAsync(
            "/api/students/profile",
            createRequest);

        // Assert - create
        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        // Act - get
        var getResponse = await _client.GetAsync(
            "/api/students/profile");

        // Assert - get
        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var profileBeforeUpdate =
            await getResponse.Content
                .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Belo Horizonte",
            profileBeforeUpdate
                .GetProperty("city")
                .GetString());

        Assert.Equal(
            "MG",
            profileBeforeUpdate
                .GetProperty("state")
                .GetString());

        var updateRequest = new
        {
            City = "Contagem",
            State = "MG",
            ExperienceLevel = ExperienceLevel.Experienced,
            OwnsVehicle = true,
            HasOwnVehicleForLessons = true
        };

        // Act - update
        var updateResponse = await _client.PutAsJsonAsync(
            "/api/students/profile",
            updateRequest);

        // Assert - update
        Assert.Equal(
            HttpStatusCode.OK,
            updateResponse.StatusCode);

        // Act - get again
        var finalGetResponse = await _client.GetAsync(
            "/api/students/profile");

        Assert.Equal(
            HttpStatusCode.OK,
            finalGetResponse.StatusCode);

        var profileAfterUpdate =
            await finalGetResponse.Content
                .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Contagem",
            profileAfterUpdate
                .GetProperty("city")
                .GetString());

        Assert.Equal(
            "MG",
            profileAfterUpdate
                .GetProperty("state")
                .GetString());

        Assert.True(
            profileAfterUpdate
                .GetProperty("ownsVehicle")
                .GetBoolean());

        Assert.True(
            profileAfterUpdate
                .GetProperty("hasOwnVehicleForLessons")
                .GetBoolean());
    }
}
using DriveMatch.Application.Features.Auth.Login;
using DriveMatch.Application.Features.Users.Account.GetMyAccount;
using DriveMatch.Application.Features.Users.Account.UpdateMyAccount;
using DriveMatch.Application.Features.Users.Register;
using DriveMatch.Domain.Enums;
using DriveMatch.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DriveMatch.IntegrationTests.Features.Users;

[Collection(IntegrationTestCollection.Name)]
public sealed class AccountTests
{
    private readonly DriveMatchApiFactory _factory;

    public AccountTests(
        DriveMatchApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMyAccount_WithoutToken_ShouldReturnUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/users/me");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetMyAccount_WithAuthenticatedUser_ShouldReturnOwnAccount()
    {
        using var client = _factory.CreateClient();

        var email =
            $"account-{Guid.NewGuid():N}@drivematch.test";

        const string password = "DriveMatch@123";
        const string name = "Integration Account User";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = name,
                Email = email,
                Password = password,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var registeredUser =
            await registerResponse.Content
                .ReadFromJsonAsync<RegisterUserResult>(
                    IntegrationJson.Options);

        Assert.NotNull(registeredUser);

        var loginResponse = await client.PostAsJsonAsync(
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
        Assert.False(
            string.IsNullOrWhiteSpace(loginResult.Token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        var response = await client.GetAsync(
            "/api/users/me");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var account =
            await response.Content
                .ReadFromJsonAsync<GetMyAccountResult>(
                    IntegrationJson.Options);

        Assert.NotNull(account);

        Assert.Equal(
            registeredUser.UserId,
            account.UserId);

        Assert.Equal(
            name,
            account.Name);

        Assert.Equal(
            email,
            account.Email);

        Assert.Equal(
            UserRole.Student,
            account.Role);
    }

    [Fact]
    public async Task GetMyAccount_ShouldNotExposePasswordHash()
    {
        using var client = _factory.CreateClient();

        var email =
            $"account-security-{Guid.NewGuid():N}@drivematch.test";

        const string password = "DriveMatch@123";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "Integration Security User",
                Email = email,
                Password = password,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
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

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        var response = await client.GetAsync(
            "/api/users/me");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var json =
            await response.Content.ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(json);

        var root = document.RootElement;

        Assert.False(
            root.TryGetProperty(
                "passwordHash",
                out _));

        Assert.False(
            root.TryGetProperty(
                "password",
                out _));
    }

    [Fact]
    public async Task UpdateMyAccount_WithoutToken_ShouldReturnUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/users/me",
            new
            {
                Name = "Updated Name",
                Email = "updated@drivematch.test"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateMyAccount_WithAuthenticatedUser_ShouldUpdateAccount()
    {
        using var client = _factory.CreateClient();

        var originalEmail =
            $"account-update-{Guid.NewGuid():N}@drivematch.test";

        var updatedEmail =
            $"account-updated-{Guid.NewGuid():N}@drivematch.test";

        const string password = "DriveMatch@123";
        const string updatedName = "Updated Account User";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "Original Account User",
                Email = originalEmail,
                Password = password,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = originalEmail,
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

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        var updateResponse = await client.PutAsJsonAsync(
            "/api/users/me",
            new
            {
                Name = updatedName,
                Email = updatedEmail
            });

        Assert.Equal(
            HttpStatusCode.OK,
            updateResponse.StatusCode);

        var updatedAccount =
            await updateResponse.Content
                .ReadFromJsonAsync<UpdateMyAccountResult>(
                    IntegrationJson.Options);

        Assert.NotNull(updatedAccount);

        Assert.Equal(
            updatedName,
            updatedAccount.Name);

        Assert.Equal(
            updatedEmail,
            updatedAccount.Email);

        var getResponse = await client.GetAsync(
            "/api/users/me");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var persistedAccount =
            await getResponse.Content
                .ReadFromJsonAsync<GetMyAccountResult>(
                    IntegrationJson.Options);

        Assert.NotNull(persistedAccount);

        Assert.Equal(
            updatedAccount.UserId,
            persistedAccount.UserId);

        Assert.Equal(
            updatedName,
            persistedAccount.Name);

        Assert.Equal(
            updatedEmail,
            persistedAccount.Email);
    }

    [Fact]
    public async Task UpdateMyAccount_WhenEmailBelongsToAnotherUser_ShouldReturnConflict()
    {
        using var client = _factory.CreateClient();

        var firstEmail =
            $"account-first-{Guid.NewGuid():N}@drivematch.test";

        var secondEmail =
            $"account-second-{Guid.NewGuid():N}@drivematch.test";

        const string password = "DriveMatch@123";

        var firstRegisterResponse = await client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "First User",
                Email = firstEmail,
                Password = password,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            firstRegisterResponse.StatusCode);

        var secondRegisterResponse = await client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "Second User",
                Email = secondEmail,
                Password = password,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            secondRegisterResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = firstEmail,
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

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        var updateResponse = await client.PutAsJsonAsync(
            "/api/users/me",
            new
            {
                Name = "First User Updated",
                Email = secondEmail
            });

        Assert.Equal(
            HttpStatusCode.Conflict,
            updateResponse.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithoutToken_ShouldReturnUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/users/me/password",
            new
            {
                CurrentPassword = "current-password",
                NewPassword = "new-password"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WhenCurrentPasswordIsIncorrect_ShouldReturnBadRequest()
    {
        using var client = _factory.CreateClient();

        var email =
            $"password-invalid-{Guid.NewGuid():N}@drivematch.test";

        const string currentPassword = "DriveMatch@123";
        const string newPassword = "DriveMatch@456";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "Password Test User",
                Email = email,
                Password = currentPassword,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = currentPassword
            });

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResult>(
                    IntegrationJson.Options);

        Assert.NotNull(loginResult);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        var changePasswordResponse = await client.PutAsJsonAsync(
            "/api/users/me/password",
            new
            {
                CurrentPassword = "wrong-password",
                NewPassword = newPassword
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            changePasswordResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;

        var oldPasswordLoginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = currentPassword
            });

        Assert.Equal(
            HttpStatusCode.OK,
            oldPasswordLoginResponse.StatusCode);

        var newPasswordLoginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = newPassword
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            newPasswordLoginResponse.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WhenCurrentPasswordIsCorrect_ShouldChangePassword()
    {
        using var client = _factory.CreateClient();

        var email =
            $"password-change-{Guid.NewGuid():N}@drivematch.test";

        const string currentPassword = "DriveMatch@123";
        const string newPassword = "DriveMatch@456";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/",
            new
            {
                Name = "Password Change User",
                Email = email,
                Password = currentPassword,
                Role = UserRole.Student
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = currentPassword
            });

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResult>(
                    IntegrationJson.Options);

        Assert.NotNull(loginResult);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        var changePasswordResponse = await client.PutAsJsonAsync(
            "/api/users/me/password",
            new
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            changePasswordResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;

        var oldPasswordLoginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = currentPassword
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            oldPasswordLoginResponse.StatusCode);

        var newPasswordLoginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = newPassword
            });

        Assert.Equal(
            HttpStatusCode.OK,
            newPasswordLoginResponse.StatusCode);
    }
}
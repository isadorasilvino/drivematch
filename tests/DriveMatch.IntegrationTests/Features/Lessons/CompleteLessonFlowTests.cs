using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DriveMatch.Application.Features.Auth.Login;
using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Application.Features.LessonRequests.Accept;
using DriveMatch.Application.Features.Students.CreateProfile;
using DriveMatch.Domain.Enums;
using DriveMatch.IntegrationTests.Infrastructure;

namespace DriveMatch.IntegrationTests.Features.Lessons;

[Collection(IntegrationTestCollection.Name)]
public sealed class CompleteLessonFlowTests
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

    private readonly DriveMatchApiFactory _factory;

    public CompleteLessonFlowTests(
        DriveMatchApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CompleteLessonFlow_ShouldSucceed()
    {
        using var instructorClient = _factory.CreateClient();
        using var studentClient = _factory.CreateClient();

        var suffix = Guid.NewGuid().ToString("N");

        var instructorEmail =
            $"e2e-instructor-{suffix}@drivematch.test";

        var studentEmail =
            $"e2e-student-{suffix}@drivematch.test";

        const string password = "DriveMatch@123";

        // =====================================================
        // 1. REGISTER INSTRUCTOR
        // =====================================================

        var instructorRegisterResponse =
            await instructorClient.PostAsJsonAsync(
                "/api/users/",
                new
                {
                    Name = "E2E Instructor",
                    Email = instructorEmail,
                    Password = password,
                    Role = UserRole.Instructor
                });

        Assert.Equal(
            HttpStatusCode.Created,
            instructorRegisterResponse.StatusCode);

        // =====================================================
        // 2. LOGIN INSTRUCTOR
        // =====================================================

        var instructorLoginResponse =
            await instructorClient.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = instructorEmail,
                    Password = password
                });

        Assert.Equal(
            HttpStatusCode.OK,
            instructorLoginResponse.StatusCode);

        var instructorLogin =
            await instructorLoginResponse.Content
                .ReadFromJsonAsync<LoginResult>(JsonOptions);

        Assert.NotNull(instructorLogin);
        Assert.False(
            string.IsNullOrWhiteSpace(instructorLogin.Token));

        instructorClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                instructorLogin.Token);

        // =====================================================
        // 3. CREATE INSTRUCTOR PROFILE
        // =====================================================

        var instructorProfileResponse =
            await instructorClient.PostAsJsonAsync(
                "/api/instructors/profile",
                new
                {
                    Description =
                        "Instrutor criado pelo teste E2E.",
                    ExperienceYears = 5,
                    City = "Belo Horizonte",
                    State = "MG",
                    PricePerLesson = 100m,
                    AcceptsBeginners = true,
                    AcceptsExperiencedStudents = true,
                    AcceptsStudentVehicle = true
                });

        Assert.Equal(
            HttpStatusCode.Created,
            instructorProfileResponse.StatusCode);

        var instructorProfile =
            await instructorProfileResponse.Content
                .ReadFromJsonAsync<CreateInstructorProfileResult>(
                    JsonOptions);

        Assert.NotNull(instructorProfile);
        Assert.NotEqual(
            Guid.Empty,
            instructorProfile.InstructorProfileId);

        // =====================================================
        // 4. CREATE AVAILABILITY
        // =====================================================

        var lessonDate =
            DateOnly.FromDateTime(
                DateTime.Today.AddDays(7));

        var startTime = new TimeOnly(10, 0);
        var endTime = new TimeOnly(11, 0);

        var availabilityResponse =
            await instructorClient.PostAsJsonAsync(
                "/api/availabilities/",
                new
                {
                    DayOfWeek = lessonDate.DayOfWeek,
                    StartTime = startTime,
                    EndTime = new TimeOnly(12, 0),
                    LessonDurationMinutes = 60,
                    BreakDurationMinutes = 0
                });

        Assert.Equal(
            HttpStatusCode.Created,
            availabilityResponse.StatusCode);

        // =====================================================
        // 5. ACTIVATE INSTRUCTOR PROFILE
        // =====================================================

        var activateResponse =
            await instructorClient.PatchAsJsonAsync(
                "/api/instructors/profile/status",
                new
                {
                    IsActive = true
                });

        Assert.Equal(
            HttpStatusCode.OK,
            activateResponse.StatusCode);
        // =====================================================
        // 6. REGISTER STUDENT
        // =====================================================

        var studentRegisterResponse =
            await studentClient.PostAsJsonAsync(
                "/api/users/",
                new
                {
                    Name = "E2E Student",
                    Email = studentEmail,
                    Password = password,
                    Role = UserRole.Student
                });

        Assert.Equal(
            HttpStatusCode.Created,
            studentRegisterResponse.StatusCode);

        // =====================================================
        // 7. LOGIN STUDENT
        // =====================================================

        var studentLoginResponse =
            await studentClient.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = studentEmail,
                    Password = password
                });

        Assert.Equal(
            HttpStatusCode.OK,
            studentLoginResponse.StatusCode);

        var studentLogin =
            await studentLoginResponse.Content
                .ReadFromJsonAsync<LoginResult>(JsonOptions);

        Assert.NotNull(studentLogin);
        Assert.False(
            string.IsNullOrWhiteSpace(studentLogin.Token));

        studentClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                studentLogin.Token);

        // =====================================================
        // 8. CREATE STUDENT PROFILE
        // =====================================================

        var studentProfileResponse =
            await studentClient.PostAsJsonAsync(
                "/api/students/profile",
                new
                {
                    City = "Belo Horizonte",
                    State = "MG",
                    ExperienceLevel =
                        ExperienceLevel.Beginner,
                    OwnsVehicle = false,
                    HasOwnVehicleForLessons = false
                });

        Assert.Equal(
            HttpStatusCode.Created,
            studentProfileResponse.StatusCode);

        var studentProfile =
            await studentProfileResponse.Content
                .ReadFromJsonAsync<CreateStudentProfileResult>(
                    JsonOptions);

        Assert.NotNull(studentProfile);
        Assert.NotEqual(
            Guid.Empty,
            studentProfile.StudentProfileId);

        // =====================================================
        // 9. CREATE LESSON REQUEST
        // =====================================================

        var lessonRequestResponse =
            await studentClient.PostAsJsonAsync(
                "/api/lessons/",
                new
                {
                    InstructorProfileId =
                        instructorProfile.InstructorProfileId,

                    RequestedDate = lessonDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    UsesStudentVehicle = false,
                    StudentMessage =
                        "Solicitação criada pelo teste E2E."
                });

        Assert.Equal(
            HttpStatusCode.Created,
            lessonRequestResponse.StatusCode);

        using var lessonRequestJson =
            JsonDocument.Parse(
                await lessonRequestResponse.Content
                    .ReadAsStringAsync());

        var lessonRequestId =
            lessonRequestJson.RootElement
                .GetProperty("lessonRequestId")
                .GetGuid();

        Assert.NotEqual(
            Guid.Empty,
            lessonRequestId);

        // =====================================================
        // 10. ACCEPT LESSON REQUEST
        // =====================================================

        var acceptResponse =
            await instructorClient.PatchAsync(
                $"/api/lessons/{lessonRequestId}/accept",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            acceptResponse.StatusCode);

        var acceptedRequest =
            await acceptResponse.Content
                .ReadFromJsonAsync<AcceptLessonRequestResult>(
                    JsonOptions);

        Assert.NotNull(acceptedRequest);

        Assert.NotEqual(
            Guid.Empty,
            acceptedRequest.LessonId);

        var lessonId =
            acceptedRequest.LessonId;

        // =====================================================
        // 11. START CHECK-IN
        // =====================================================

        var startCheckInResponse =
            await instructorClient.PatchAsync(
                $"/api/lessons/{lessonId}/check-in/start",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            startCheckInResponse.StatusCode);

        using var startCheckInJson =
            JsonDocument.Parse(
                await startCheckInResponse.Content
                    .ReadAsStringAsync());

        var checkInToken =
            startCheckInJson.RootElement
                .GetProperty("checkInToken")
                .GetString();

        Assert.False(
            string.IsNullOrWhiteSpace(checkInToken));

        // =====================================================
        // 12. CONFIRM CHECK-IN
        // =====================================================

        var confirmCheckInResponse =
            await studentClient.PatchAsJsonAsync(
                $"/api/lessons/{lessonId}/check-in/confirm",
                new
                {
                    CheckInToken = checkInToken
                });

        Assert.Equal(
            HttpStatusCode.OK,
            confirmCheckInResponse.StatusCode);

        // =====================================================
        // 13. COMPLETE LESSON
        // =====================================================

        var completeResponse =
            await instructorClient.PatchAsync(
                $"/api/lessons/{lessonId}/complete",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            completeResponse.StatusCode);

        // =====================================================
        // 14. CREATE REVIEW
        // =====================================================

        var reviewResponse =
            await studentClient.PostAsJsonAsync(
                "/api/reviews/",
                new
                {
                    LessonId = lessonId,
                    Rating = 5,
                    Comment =
                        "Avaliação criada pelo teste E2E."
                });

        Assert.Equal(
            HttpStatusCode.Created,
            reviewResponse.StatusCode);
    }
}

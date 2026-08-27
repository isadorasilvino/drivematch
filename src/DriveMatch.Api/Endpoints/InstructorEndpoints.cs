using DriveMatch.Application.Features.Instructors.ChangeStatus;
using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Application.Features.Instructors.Search;
using DriveMatch.Application.Features.Instructors.UpdateProfile;
using DriveMatch.Api.Extensions;
using System.Security.Claims;
using DriveMatch.Domain.Enums;

using ChangeStatusInstructorNotFoundException =
    DriveMatch.Application.Features.Instructors.ChangeStatus.InstructorProfileNotFoundException;

using UpdateInstructorNotFoundException =
    DriveMatch.Application.Features.Instructors.UpdateProfile.InstructorProfileNotFoundException;

namespace DriveMatch.Api.Endpoints;

public static class InstructorEndpoints
{
    public static IEndpointRouteBuilder MapInstructorEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/instructors")
            .WithTags("Instructors");

        group.MapPost("/profile", CreateProfileAsync)
            .WithName("CreateInstructorProfile")
            .Produces<CreateInstructorProfileResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(policy =>
                policy.RequireRole("Instructor")); ;

        group.MapPut("/profile", UpdateProfileAsync)
            .WithName("UpdateInstructorProfile")
            .Produces<UpdateInstructorProfileResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/profile/status", ChangeStatusAsync)
            .WithName("ChangeInstructorProfileStatus")
            .Produces<ChangeInstructorProfileStatusResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/search", SearchAsync)
            .WithName("SearchInstructors")
            .Produces<IReadOnlyCollection<SearchInstructorResult>>(
                StatusCodes.Status200OK);

        return endpoints;
    }

    private static async Task<IResult> CreateProfileAsync(
        ClaimsPrincipal user,
        CreateInstructorProfileRequest request,
        CreateInstructorProfileHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await handler.HandleAsync(
                new CreateInstructorProfileCommand(
                    userId,
                    request.Description,
                    request.ExperienceYears,
                    request.City,
                    request.State,
                    request.PricePerLesson,
                    request.AcceptsBeginners,
                    request.AcceptsExperiencedStudents,
                    request.AcceptsStudentVehicle),
                cancellationToken);

            return Results.Created(
                $"/api/instructors/profile/{result.InstructorProfileId}",
                result);
        }
        catch (UserNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (InvalidUserRoleException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
        catch (InstructorProfileAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateProfileAsync(
        ClaimsPrincipal user,
        UpdateInstructorProfileRequest request,
        UpdateInstructorProfileHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new UpdateInstructorProfileCommand(
                    userId,
                    request.Description,
                    request.ExperienceYears,
                    request.City,
                    request.State,
                    request.PricePerLesson,
                    request.AcceptsBeginners,
                    request.AcceptsExperiencedStudents,
                    request.AcceptsStudentVehicle),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (UpdateInstructorNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    private static async Task<IResult> ChangeStatusAsync(
        ClaimsPrincipal user,
        ChangeInstructorProfileStatusRequest request,
        ChangeInstructorProfileStatusHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await handler.HandleAsync(
                new ChangeInstructorProfileStatusCommand(
                    userId,
                    request.IsActive),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (ChangeStatusInstructorNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    private static async Task<IResult> SearchAsync(
        string city,
        string state,
        ExperienceLevel experienceLevel,
        bool usesStudentVehicle,
        decimal? maxPricePerLesson,
        SearchInstructorsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchInstructorsQuery(
                city,
                state,
                experienceLevel,
                usesStudentVehicle,
                maxPricePerLesson),
            cancellationToken);

        return Results.Ok(result);
    }

    public sealed record ChangeInstructorProfileStatusRequest(
        bool IsActive);

    public sealed record CreateInstructorProfileRequest(
        string Description,
        int ExperienceYears,
        string City,
        string State,
        decimal PricePerLesson,
        bool AcceptsBeginners,
        bool AcceptsExperiencedStudents,
        bool AcceptsStudentVehicle);

    public sealed record UpdateInstructorProfileRequest(
        string Description,
        int ExperienceYears,
        string City,
        string State,
        decimal PricePerLesson,
        bool AcceptsBeginners,
        bool AcceptsExperiencedStudents,
        bool AcceptsStudentVehicle);
}
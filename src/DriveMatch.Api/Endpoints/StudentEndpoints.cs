using DriveMatch.Api.Extensions;
using DriveMatch.Application.Features.Students.CreateProfile;
using DriveMatch.Application.Features.Students.UpdateProfile;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using System.Security.Claims;

namespace DriveMatch.Api.Endpoints;

public static class StudentEndpoints
{
    public static IEndpointRouteBuilder MapStudentEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/students")
            .WithTags("Students")
            .RequireAuthorization(policy =>
                policy.RequireRole("Student"));

        group.MapPost("/profile", CreateProfileAsync)
            .WithName("CreateStudentProfile")
            .Produces<CreateStudentProfileResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/profile", UpdateProfileAsync)
            .WithName("UpdateStudentProfile")
            .Produces<UpdateStudentProfileResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateProfileAsync(ClaimsPrincipal user,
        CreateStudentProfileRequest request,
        CreateStudentProfileHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new CreateStudentProfileCommand(
                    userId,
                    request.City,
                    request.State,
                    request.ExperienceLevel,
                    request.OwnsVehicle,
                    request.HasOwnVehicleForLessons),
                cancellationToken);

            return Results.Created(
                $"/api/students/profile/{result.StudentProfileId}",
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
        catch (StudentProfileAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateProfileAsync(ClaimsPrincipal user,
        UpdateStudentProfileRequest request,
        UpdateStudentProfileHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new UpdateStudentProfileCommand(
                    userId,
                    request.City,
                    request.State,
                    request.ExperienceLevel,
                    request.OwnsVehicle,
                    request.HasOwnVehicleForLessons),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (StudentProfileNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    public sealed record CreateStudentProfileRequest(
        string City,
        string State,
        ExperienceLevel ExperienceLevel,
        bool OwnsVehicle,
        bool HasOwnVehicleForLessons);


    public sealed record UpdateStudentProfileRequest(
        string City,
        string State,
        ExperienceLevel ExperienceLevel,
        bool OwnsVehicle,
        bool HasOwnVehicleForLessons);
}
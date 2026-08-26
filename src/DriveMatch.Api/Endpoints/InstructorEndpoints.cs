using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Application.Features.Instructors.UpdateProfile;

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
            .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/profile", UpdateProfileAsync)
            .WithName("UpdateInstructorProfile")
            .Produces<UpdateInstructorProfileResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateProfileAsync(
        CreateInstructorProfileRequest request,
        CreateInstructorProfileHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CreateInstructorProfileCommand(
                    request.UserId,
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
        UpdateInstructorProfileRequest request,
        UpdateInstructorProfileHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new UpdateInstructorProfileCommand(
                    request.UserId,
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
        catch (InstructorProfileNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    public sealed record CreateInstructorProfileRequest(
        Guid UserId,
        string Description,
        int ExperienceYears,
        string City,
        string State,
        decimal PricePerLesson,
        bool AcceptsBeginners,
        bool AcceptsExperiencedStudents,
        bool AcceptsStudentVehicle);

    public sealed record UpdateInstructorProfileRequest(
        Guid UserId,
        string Description,
        int ExperienceYears,
        string City,
        string State,
        decimal PricePerLesson,
        bool AcceptsBeginners,
        bool AcceptsExperiencedStudents,
        bool AcceptsStudentVehicle);
}
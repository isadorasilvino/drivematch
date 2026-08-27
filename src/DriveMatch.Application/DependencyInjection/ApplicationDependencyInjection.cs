using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Application.Features.Instructors.UpdateProfile;
using DriveMatch.Application.Features.Students.CreateProfile;
using DriveMatch.Application.Features.Students.UpdateProfile;
using DriveMatch.Application.Features.Users.Register;
using DriveMatch.Application.Features.Availabilities.ChangeStatus;
using DriveMatch.Application.Features.Availabilities.Create;
using DriveMatch.Application.Features.Availabilities.Update;
using DriveMatch.Application.Features.Instructors.Search;
using DriveMatch.Application.Features.Instructors.ChangeStatus;
using DriveMatch.Application.Features.LessonRequests.Create;
using DriveMatch.Application.Features.LessonRequests.Accept;
using DriveMatch.Application.Features.LessonRequests.Reject;
using DriveMatch.Application.Features.Lessons.Cancel;
using DriveMatch.Application.Features.Lessons.Complete;
using DriveMatch.Application.Features.Lessons.ConfirmCheckIn;
using DriveMatch.Application.Features.Lessons.MarkAsNotAttended;
using DriveMatch.Application.Features.Lessons.StartCheckIn;
using Microsoft.Extensions.DependencyInjection;

namespace DriveMatch.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();

        services.AddScoped<CreateStudentProfileHandler>();
        services.AddScoped<UpdateStudentProfileHandler>();

        services.AddScoped<CreateInstructorProfileHandler>();
        services.AddScoped<UpdateInstructorProfileHandler>();
        services.AddScoped<CreateAvailabilityHandler>();
        services.AddScoped<UpdateAvailabilityHandler>();
        services.AddScoped<ChangeAvailabilityStatusHandler>();
        services.AddScoped<SearchInstructorsHandler>();
        services.AddScoped<ChangeInstructorProfileStatusHandler>();
        services.AddScoped<CreateLessonRequestHandler>();
        services.AddScoped<AcceptLessonRequestHandler>();
        services.AddScoped<RejectLessonRequestHandler>();
        services.AddScoped<StartLessonCheckInHandler>();
        services.AddScoped<ConfirmLessonCheckInHandler>();
        services.AddScoped<CompleteLessonHandler>();
        services.AddScoped<CancelLessonHandler>();
        services.AddScoped<MarkLessonAsNotAttendedHandler>();

        return services;
    }
}
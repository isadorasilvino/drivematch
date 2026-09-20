using DriveMatch.Application.Features.Auth.Login;
using DriveMatch.Application.Features.Availabilities.ChangeStatus;
using DriveMatch.Application.Features.Availabilities.Create;
using DriveMatch.Application.Features.Availabilities.GetAvailableSlots;
using DriveMatch.Application.Features.Availabilities.GetMine;
using DriveMatch.Application.Features.Availabilities.GetNextAvailableDate;
using DriveMatch.Application.Features.Availabilities.Update;
using DriveMatch.Application.Features.Instructors.ChangeStatus;
using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Application.Features.Instructors.GetProfile;
using DriveMatch.Application.Features.Instructors.Search;
using DriveMatch.Application.Features.Instructors.UpdateProfile;
using DriveMatch.Application.Features.LessonRequests.Accept;
using DriveMatch.Application.Features.LessonRequests.Cancel;
using DriveMatch.Application.Features.LessonRequests.Create;
using DriveMatch.Application.Features.LessonRequests.GetMine;
using DriveMatch.Application.Features.LessonRequests.GetReceived;
using DriveMatch.Application.Features.LessonRequests.Reject;
using DriveMatch.Application.Features.Lessons.Cancel;
using DriveMatch.Application.Features.Lessons.Complete;
using DriveMatch.Application.Features.Lessons.ConfirmCheckIn;
using DriveMatch.Application.Features.Lessons.GetInstructorLessons;
using DriveMatch.Application.Features.Lessons.GetMine;
using DriveMatch.Application.Features.Lessons.MarkAsNotAttended;
using DriveMatch.Application.Features.Lessons.StartCheckIn;
using DriveMatch.Application.Features.Reviews.Create;
using DriveMatch.Application.Features.Students.CreateProfile;
using DriveMatch.Application.Features.Students.GetProfile;
using DriveMatch.Application.Features.Students.UpdateProfile;
using DriveMatch.Application.Features.Users.Account.ChangePassword;
using DriveMatch.Application.Features.Users.Account.GetMyAccount;
using DriveMatch.Application.Features.Users.Account.UpdateMyAccount;
using DriveMatch.Application.Features.Users.Register;
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
        services.AddScoped<GetInstructorProfileHandler>();
        services.AddScoped<CreateLessonRequestHandler>();
        services.AddScoped<AcceptLessonRequestHandler>();
        services.AddScoped<RejectLessonRequestHandler>();
        services.AddScoped<CancelLessonRequestHandler>();
        services.AddScoped<StartLessonCheckInHandler>();
        services.AddScoped<ConfirmLessonCheckInHandler>();
        services.AddScoped<CompleteLessonHandler>();
        services.AddScoped<CancelLessonHandler>();
        services.AddScoped<MarkLessonAsNotAttendedHandler>();
        services.AddScoped<CreateReviewHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<GetStudentProfileHandler>();
        services.AddScoped<GetMyAvailabilitiesHandler>();
        services.AddScoped<GetAvailableSlotsHandler>();
        services.AddScoped<GetNextAvailableDateHandler>();
        services.AddScoped<GetMineLessonRequestsHandler>();
        services.AddScoped<GetReceivedLessonRequestsHandler>();
        services.AddScoped<GetMineLessonsHandler>();
        services.AddScoped<GetInstructorLessonsHandler>();
        services.AddScoped<GetMyAccountHandler>();
        services.AddScoped<UpdateMyAccountHandler>();
        services.AddScoped<ChangePasswordHandler>();

        return services;
    }
}
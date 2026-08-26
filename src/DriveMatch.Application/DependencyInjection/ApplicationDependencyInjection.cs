using DriveMatch.Application.Features.Users.Register;
using Microsoft.Extensions.DependencyInjection;

namespace DriveMatch.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();

        return services;
    }
}

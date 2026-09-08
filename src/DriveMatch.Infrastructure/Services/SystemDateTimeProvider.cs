using DriveMatch.Application.Abstractions.Time;

namespace DriveMatch.Infrastructure.Services;

public sealed class SystemDateTimeProvider(
    TimeZoneInfo timeZone) : IDateTimeProvider
{
    public DateTime LocalNow =>
        TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            timeZone);
}

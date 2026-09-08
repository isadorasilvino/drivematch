namespace DriveMatch.Application.Abstractions.Time;

public interface IDateTimeProvider
{
    DateTime LocalNow { get; }
}
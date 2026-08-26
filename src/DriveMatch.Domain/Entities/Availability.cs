using DriveMatch.Domain.Common;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Entities;

public class Availability : Entity
{
    public Guid InstructorProfileId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsActive { get; private set; }

    private Availability()
    {
    }

    public Availability(
        Guid id,
        Guid instructorProfileId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime)
        : base(id)
    {
        if (instructorProfileId == Guid.Empty)
            throw new DomainException("O identificador do perfil do instrutor deve ser informado.");

        ValidateTimeRange(startTime, endTime);

        InstructorProfileId = instructorProfileId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        IsActive = true;
    }

    public void Update(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        ValidateTimeRange(startTime, endTime);

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static void ValidateTimeRange(
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (startTime >= endTime)
            throw new DomainException(
                "O horário inicial deve ser anterior ao horário final.");
    }
}

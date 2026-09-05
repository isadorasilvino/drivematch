using DriveMatch.Domain.Common;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Entities;

public class Availability : Entity
{
    private static readonly int[] AllowedLessonDurations =
    [
        30,
        40,
        45,
        50,
        60
    ];

    private static readonly int[] AllowedBreakDurations =
    [
        0,
        5,
        10,
        15,
        20,
        30
    ];

    public Guid InstructorProfileId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int LessonDurationMinutes { get; private set; }
    public int BreakDurationMinutes { get; private set; }
    public bool IsActive { get; private set; }

    private Availability()
    {
    }

    public Availability(
        Guid id,
        Guid instructorProfileId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int lessonDurationMinutes,
        int breakDurationMinutes)
        : base(id)
    {
        if (instructorProfileId == Guid.Empty)
            throw new DomainException(
                "O identificador do perfil do instrutor deve ser informado.");

        Validate(
            startTime,
            endTime,
            lessonDurationMinutes,
            breakDurationMinutes);

        InstructorProfileId = instructorProfileId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        LessonDurationMinutes = lessonDurationMinutes;
        BreakDurationMinutes = breakDurationMinutes;
        IsActive = true;
    }

    public void Update(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int lessonDurationMinutes,
        int breakDurationMinutes)
    {
        Validate(
            startTime,
            endTime,
            lessonDurationMinutes,
            breakDurationMinutes);

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        LessonDurationMinutes = lessonDurationMinutes;
        BreakDurationMinutes = breakDurationMinutes;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static void Validate(
        TimeOnly startTime,
        TimeOnly endTime,
        int lessonDurationMinutes,
        int breakDurationMinutes)
    {
        if (startTime >= endTime)
        {
            throw new DomainException(
                "O horário inicial deve ser anterior ao horário final.");
        }

        if (!AllowedLessonDurations.Contains(lessonDurationMinutes))
        {
            throw new DomainException(
                "A duração da aula deve ser 30, 40, 45, 50 ou 60 minutos.");
        }

        if (!AllowedBreakDurations.Contains(breakDurationMinutes))
        {
            throw new DomainException(
                "O intervalo entre aulas deve ser 0, 5, 10, 15, 20 ou 30 minutos.");
        }

        var availabilityDuration =
            endTime.ToTimeSpan() - startTime.ToTimeSpan();

        if (availabilityDuration.TotalMinutes < lessonDurationMinutes)
        {
            throw new DomainException(
                "A disponibilidade deve comportar pelo menos uma aula completa.");
        }
    }
}
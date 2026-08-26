using DriveMatch.Domain.Common;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Entities;

public class Lesson : Entity
{
    public Guid StudentId { get; private set; }
    public Guid InstructorId { get; private set; }
    public Guid LessonRequestId { get; private set; }
    public DateOnly ScheduledDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public LessonStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CheckInAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Lesson()
    {
    }

    public Lesson(
        Guid id,
        Guid studentId,
        Guid instructorId,
        Guid lessonRequestId,
        DateOnly scheduledDate,
        TimeOnly startTime,
        TimeOnly endTime)
        : base(id)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("O identificador do aluno deve ser informado.");

        if (instructorId == Guid.Empty)
            throw new DomainException("O identificador do instrutor deve ser informado.");

        if (lessonRequestId == Guid.Empty)
            throw new DomainException("O identificador da solicitação de aula deve ser informado.");

        if (studentId == instructorId)
            throw new DomainException("Aluno e instrutor devem ser usuários diferentes.");

        ValidateTimeRange(startTime, endTime);

        StudentId = studentId;
        InstructorId = instructorId;
        LessonRequestId = lessonRequestId;
        ScheduledDate = scheduledDate;
        StartTime = startTime;
        EndTime = endTime;
        Status = LessonStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public void StartCheckIn()
    {
        EnsureStatus(LessonStatus.Scheduled);

        Status = LessonStatus.CheckIn;
    }

    public void ConfirmCheckIn()
    {
        EnsureStatus(LessonStatus.CheckIn);

        CheckInAt = DateTime.UtcNow;
        StartedAt = DateTime.UtcNow;
        Status = LessonStatus.InProgress;
    }

    public void Complete()
    {
        EnsureStatus(LessonStatus.InProgress);

        CompletedAt = DateTime.UtcNow;
        Status = LessonStatus.Completed;
    }

    public void Cancel()
    {
        EnsureStatus(LessonStatus.Scheduled);

        CancelledAt = DateTime.UtcNow;
        Status = LessonStatus.Cancelled;
    }

    public void MarkAsNotAttended()
    {
        EnsureStatus(LessonStatus.Scheduled);

        Status = LessonStatus.NotAttended;
    }

    private void EnsureStatus(LessonStatus expectedStatus)
    {
        if (Status != expectedStatus)
            throw new DomainException(
                $"A aula não pode executar esta operação no status atual: {Status}.");
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

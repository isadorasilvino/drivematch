using DriveMatch.Domain.Common;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Entities;

public class LessonRequest : Entity
{
    public Guid StudentId { get; private set; }
    public Guid InstructorId { get; private set; }
    public DateOnly RequestedDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool UsesStudentVehicle { get; private set; }
    public string? StudentMessage { get; private set; }
    public LessonRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private LessonRequest()
    {
    }

    public LessonRequest(
        Guid id,
        Guid studentId,
        Guid instructorId,
        DateOnly requestedDate,
        TimeOnly startTime,
        TimeOnly endTime,
        bool usesStudentVehicle,
        string? studentMessage)
        : base(id)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("O identificador do aluno deve ser informado.");

        if (instructorId == Guid.Empty)
            throw new DomainException("O identificador do instrutor deve ser informado.");

        if (studentId == instructorId)
            throw new DomainException("Aluno e instrutor devem ser usuários diferentes.");

        ValidateTimeRange(startTime, endTime);

        StudentId = studentId;
        InstructorId = instructorId;
        RequestedDate = requestedDate;
        StartTime = startTime;
        EndTime = endTime;
        UsesStudentVehicle = usesStudentVehicle;
        StudentMessage = NormalizeMessage(studentMessage);
        Status = LessonRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Accept()
    {
        EnsureStatus(LessonRequestStatus.Pending);

        Status = LessonRequestStatus.Accepted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        EnsureStatus(LessonRequestStatus.Accepted);

        Status = LessonRequestStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        EnsureStatus(LessonRequestStatus.Pending);

        Status = LessonRequestStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        EnsureStatus(LessonRequestStatus.Pending);

        Status = LessonRequestStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        EnsureStatus(LessonRequestStatus.Pending);

        Status = LessonRequestStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureStatus(LessonRequestStatus expectedStatus)
    {
        if (Status != expectedStatus)
            throw new DomainException(
                $"A solicitação não pode executar esta operação no status atual: {Status}.");
    }

    private static void ValidateTimeRange(
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (startTime >= endTime)
            throw new DomainException(
                "O horário inicial deve ser anterior ao horário final.");
    }

    private static string? NormalizeMessage(string? message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? null
            : message.Trim();
    }
}

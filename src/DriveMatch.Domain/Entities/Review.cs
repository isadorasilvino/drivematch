using DriveMatch.Domain.Common;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Entities;

public class Review : Entity
{
    public Guid LessonId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid InstructorId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Review()
    {
    }

    public Review(
        Guid id,
        Guid lessonId,
        Guid studentId,
        Guid instructorId,
        int rating,
        string? comment)
        : base(id)
    {
        if (lessonId == Guid.Empty)
            throw new DomainException("O identificador da aula deve ser informado.");

        if (studentId == Guid.Empty)
            throw new DomainException("O identificador do aluno deve ser informado.");

        if (instructorId == Guid.Empty)
            throw new DomainException("O identificador do instrutor deve ser informado.");

        if (studentId == instructorId)
            throw new DomainException("Aluno e instrutor devem ser usuários diferentes.");

        ValidateRating(rating);

        LessonId = lessonId;
        StudentId = studentId;
        InstructorId = instructorId;
        Rating = rating;
        Comment = NormalizeComment(comment);
        CreatedAt = DateTime.UtcNow;
    }

    private static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("A avaliação deve possuir nota entre 1 e 5.");
    }

    private static string? NormalizeComment(string? comment)
    {
        return string.IsNullOrWhiteSpace(comment)
            ? null
            : comment.Trim();
    }
}

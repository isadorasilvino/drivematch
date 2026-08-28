using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Reviews;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Reviews.Create;

public sealed class CreateReviewHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IStudentProfileRepository _studentProfileRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewHandler(
        ILessonRepository lessonRepository,
        IStudentProfileRepository studentProfileRepository,
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _studentProfileRepository = studentProfileRepository;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateReviewResult> HandleAsync(
        CreateReviewCommand command,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            command.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new LessonNotFoundException(command.LessonId);

        var studentProfile =
            await _studentProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (studentProfile is null ||
            studentProfile.Id != lesson.StudentId)
        {
            throw new ReviewForbiddenException();
        }

        if (lesson.Status != LessonStatus.Completed)
            throw new LessonNotCompletedException(command.LessonId);

        var reviewAlreadyExists =
            await _reviewRepository.ExistsForLessonAsync(
                lesson.Id,
                cancellationToken);

        if (reviewAlreadyExists)
            throw new ReviewAlreadyExistsException(lesson.Id);

        var review = new Review(
            Guid.NewGuid(),
            lesson.Id,
            lesson.StudentId,
            lesson.InstructorId,
            command.Rating,
            command.Comment);

        await _reviewRepository.AddAsync(
            review,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateReviewResult(
            review.Id,
            review.LessonId,
            review.StudentId,
            review.InstructorId,
            review.Rating,
            review.Comment,
            review.CreatedAt);
    }
}
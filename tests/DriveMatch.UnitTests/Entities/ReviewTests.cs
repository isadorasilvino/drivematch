using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.UnitTests.Entities;

public class ReviewTests
{
    [Fact]
    public void Constructor_ShouldCreateReview_WhenDataIsValid()
    {
        var lessonId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();

        var review = new Review(
            Guid.NewGuid(),
            lessonId,
            studentId,
            instructorId,
            5,
            " Excelente instrutor. ");

        Assert.Equal(lessonId, review.LessonId);
        Assert.Equal(studentId, review.StudentId);
        Assert.Equal(instructorId, review.InstructorId);
        Assert.Equal(5, review.Rating);
        Assert.Equal("Excelente instrutor.", review.Comment);
        Assert.NotEqual(default, review.CreatedAt);
    }

    [Fact]
    public void Constructor_ShouldNormalizeEmptyCommentToNull()
    {
        var review = CreateReview(5, " ");

        Assert.Null(review.Comment);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenLessonIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new Review(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                Guid.NewGuid(),
                5,
                null));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStudentIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new Review(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                5,
                null));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenInstructorIdIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            new Review(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                5,
                null));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenStudentAndInstructorAreSame()
    {
        var userId = Guid.NewGuid();

        Assert.Throws<DomainException>(() =>
            new Review(
                Guid.NewGuid(),
                Guid.NewGuid(),
                userId,
                userId,
                5,
                null));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenRatingIsLowerThanOne()
    {
        Assert.Throws<DomainException>(() =>
            CreateReview(0));
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenRatingIsGreaterThanFive()
    {
        Assert.Throws<DomainException>(() =>
            CreateReview(6));
    }

    [Fact]
    public void Constructor_ShouldAllowAllValidRatings()
    {
        for (var rating = 1; rating <= 5; rating++)
        {
            var review = CreateReview(rating);

            Assert.Equal(rating, review.Rating);
        }
    }

    private static Review CreateReview(
        int rating = 5,
        string? comment = null)
    {
        return new Review(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            rating,
            comment);
    }
}

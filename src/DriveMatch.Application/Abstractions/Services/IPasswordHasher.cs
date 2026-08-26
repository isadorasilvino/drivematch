namespace DriveMatch.Application.Abstractions.Services;

public interface IPasswordHasher
{
    string Hash(string password);
}

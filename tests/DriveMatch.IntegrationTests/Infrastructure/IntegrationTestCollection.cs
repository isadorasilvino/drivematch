namespace DriveMatch.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection
    : ICollectionFixture<DriveMatchApiFactory>
{
    public const string Name = "DriveMatch Integration Tests";
}

using MyAppRoot.TestData;

namespace IntegrationTests.BaseReadOnlyRepository;

public class FindByPredicateSqlServer
{
    [Test]
    public async Task SqlServerDatabaseIsNotCaseSensitive()
    {
        using var repositoryHelper = RepositoryHelper.CreateSqlServerRepositoryHelper(this);
        using var repository = repositoryHelper.GetOfficeRepository();
        var item = OfficeData.GetOffices.First(e => e.Active);

        // Test using a predicate that compares an uppercase name to a lowercase name.
        var result = await repository.FindAsync(e =>
            e.Name.ToUpper().Equals(item.Name.ToLower()));

        result.Should().BeEquivalentTo(item);
    }
}

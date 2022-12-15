using FluentAssertions.Execution;
using MyAppRoot.Domain.Offices;
using MyAppRoot.TestData;
using MyAppRoot.TestData.Constants;

namespace IntegrationTests.BaseReadOnlyRepository;

public class FindByPredicate
{
    private IOfficeRepository _repository = default!;

    [SetUp]
    public void SetUp() => _repository = RepositoryHelper.CreateRepositoryHelper().GetOfficeRepository();

    [TearDown]
    public void TearDown() => _repository.Dispose();

    [Test]
    public async Task WhenItemExists_ReturnsItem()
    {
        var item = OfficeData.GetOffices.First(e => e.Active);
        var result = await _repository.FindAsync(e => e.Name == item.Name);
        result.Should().BeEquivalentTo(item);
    }

    [Test]
    public async Task SqliteDatabaseIsCaseSensitive()
    {
        var item = OfficeData.GetOffices.First(e => e.Active);

        var resultIgnoreCase = await _repository.FindAsync(e =>
            e.Name.ToLower().Equals(item.Name.ToLower()));
        var resultCaseSensitive = await _repository.FindAsync(e =>
            e.Name.Equals(item.Name.ToLower()));

        using (new AssertionScope())
        {
            resultIgnoreCase.Should().BeEquivalentTo(item);
            resultCaseSensitive.Should().BeNull();
        }
    }

    [Test]
    public async Task WhenDoesNotExist_ReturnsNull()
    {
        var result = await _repository.FindAsync(e => e.Name == TestConstants.NonExistentName);
        result.Should().BeNull();
    }
}

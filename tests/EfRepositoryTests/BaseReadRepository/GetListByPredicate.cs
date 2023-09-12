using FluentAssertions.Execution;
using MyApp.Domain.Entities.Offices;
using MyApp.TestData;
using MyApp.TestData.Constants;

namespace EfRepositoryTests.BaseReadRepository;

public class GetListByPredicate
{
    private IOfficeRepository _repository = default!;

    [SetUp]
    public void SetUp() => _repository = RepositoryHelper.CreateRepositoryHelper().GetOfficeRepository();

    [TearDown]
    public void TearDown() => _repository.Dispose();

    [Test]
    public async Task WhenItemsExist_ReturnsList()
    {
        var item = OfficeData.GetOffices.First(e => e.Active);
        var result = await _repository.GetListAsync(e => e.Name == item.Name);
        using (new AssertionScope())
        {
            result.Count.Should().Be(1);
            result.First().Should().BeEquivalentTo(item);
        }
    }

    [Test]
    public async Task WhenDoesNotExist_ReturnsEmptyList()
    {
        var result = await _repository.GetListAsync(e => e.Name == TextData.NonExistentName);
        result.Should().BeEmpty();
    }
}

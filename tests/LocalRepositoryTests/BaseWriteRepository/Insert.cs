using FluentAssertions.Execution;
using MyApp.Domain.Entities.Offices;
using MyApp.LocalRepository.Repositories;
using MyApp.TestData.Constants;

namespace LocalRepositoryTests.BaseWriteRepository;

public class Insert
{
    private LocalOfficeRepository _repository = default!;

    [SetUp]
    public void SetUp() => _repository = RepositoryHelper.GetOfficeRepository();

    [TearDown]
    public void TearDown() => _repository.Dispose();

    [Test]
    public async Task WhenItemIsValid_InsertsItem()
    {
        var initialCount = _repository.Items.Count;
        var newItem = new Office(Guid.NewGuid(), TextData.ValidName);

        await _repository.InsertAsync(newItem);

        var getResult = await _repository.GetAsync(newItem.Id);
        using (new AssertionScope())
        {
            getResult.Should().BeEquivalentTo(newItem);
            _repository.Items.Count.Should().Be(initialCount + 1);
        }
    }
}

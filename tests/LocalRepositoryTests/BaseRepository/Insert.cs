using MyAppRoot.Domain.Entities;
using MyAppRoot.LocalRepository.Repositories;
using MyAppRoot.TestData.Constants;

namespace LocalRepositoryTests.BaseReadOnlyRepository;

public class Insert
{
    private LocalOfficeRepository _repository = default!;

    [SetUp]
    public void SetUp() => _repository = new LocalOfficeRepository();

    [TearDown]
    public void TearDown() => _repository.Dispose();

    [Test]
    public async Task WhenItemIsValid_InsertsItem()
    {
        var initialCount = _repository.Items.Count;
        var newItem = new Office(Guid.NewGuid(), TestConstants.ValidName);

        await _repository.InsertAsync(newItem);

        var getResult = await _repository.GetAsync(newItem.Id);
        Assert.Multiple(() =>
        {
            getResult.Should().BeEquivalentTo(newItem);
            _repository.Items.Count.Should().Be(initialCount + 1);
        });
    }
}

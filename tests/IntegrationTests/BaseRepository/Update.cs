using MyAppRoot.Domain.Offices;
using MyAppRoot.Domain.Entities;
using MyAppRoot.TestData.Offices;
using MyAppRoot.TestData.Constants;
using GaEpd.Library.Domain.Repositories;

namespace IntegrationTests.BaseRepository;

public class Update
{
    private RepositoryHelper _repositoryHelper = default!;
    private IOfficeRepository _repository = default!;

    [SetUp]
    public void SetUp()
    {
        _repositoryHelper = RepositoryHelper.CreateRepositoryHelper();
        _repository = _repositoryHelper.GetOfficeRepository();
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
        _repositoryHelper.Dispose();
    }

    [Test]
    public async Task WhenItemIsValid_UpdatesItem()
    {
        var item = OfficeData.GetOffices.First(e => e.Active);
        item.ChangeName(TestConstants.ValidName);
        item.Active = !item.Active;

        await _repository.UpdateAsync(item, true);
        _repositoryHelper.ClearChangeTracker();

        var getResult = await _repository.GetAsync(item.Id);
        getResult.Should().BeEquivalentTo(item);
    }

    [Test]
    public async Task WhenItemDoesNotExist_Throws()
    {
        var item = new Office(Guid.Empty, TestConstants.ValidName);
        var action = async () => await _repository.UpdateAsync(item, true);
        (await action.Should().ThrowAsync<EntityNotFoundException>())
            .WithMessage($"Entity not found. Entity type: {typeof(Office).FullName}, id: {item.Id}");
    }
}

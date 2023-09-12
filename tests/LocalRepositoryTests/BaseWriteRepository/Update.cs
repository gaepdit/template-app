using GaEpd.AppLibrary.Domain.Repositories;
using MyApp.Domain.Entities.Offices;
using MyApp.LocalRepository.Repositories;
using MyApp.TestData.Constants;

namespace LocalRepositoryTests.BaseWriteRepository;

public class Update
{
    private LocalOfficeRepository _repository = default!;

    [SetUp]
    public void SetUp() => _repository = RepositoryHelper.GetOfficeRepository();

    [TearDown]
    public void TearDown() => _repository.Dispose();

    [Test]
    public async Task WhenItemIsValid_UpdatesItem()
    {
        var item = _repository.Items.First();
        item.ChangeName(TextData.ValidName);
        item.Active = !item.Active;

        await _repository.UpdateAsync(item);

        var getResult = await _repository.GetAsync(item.Id);
        getResult.Should().BeEquivalentTo(item);
    }

    [Test]
    public async Task WhenItemDoesNotExist_Throws()
    {
        var item = new Office(Guid.Empty, TextData.ValidName);
        var action = async () => await _repository.UpdateAsync(item);
        (await action.Should().ThrowAsync<EntityNotFoundException>())
            .WithMessage($"Entity not found. Entity type: {typeof(Office).FullName}, id: {item.Id}");
    }
}

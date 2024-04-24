using AutoMapper;
using MyApp.AppServices.Offices;
using MyApp.AppServices.UserServices;
using MyApp.Domain.Entities.Offices;
using MyApp.TestData.Constants;

namespace AppServicesTests.Offices;

public class FindForUpdate
{
    [Test]
    public async Task WhenItemExists_ReturnsViewDto()
    {
        // Arrange
        var office = new Office(Guid.Empty, TextData.ValidName);

        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindAsync(office.Id, Arg.Any<CancellationToken>()).Returns(office);

        var appService = new OfficeService(repoMock, Substitute.For<IOfficeManager>(),
            AppServicesTestsSetup.Mapper!, Substitute.For<IUserService>());

        // Act
        var result = await appService.FindForUpdateAsync(Guid.Empty);

        // Assert
        result.Should().BeEquivalentTo(office);
    }

    [Test]
    public async Task WhenDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Office?)null);

        var appService = new OfficeService(repoMock, Substitute.For<IOfficeManager>(),
            Substitute.For<IMapper>(), Substitute.For<IUserService>());

        // Act
        var result = await appService.FindForUpdateAsync(Guid.Empty);

        // Assert
        result.Should().BeNull();
    }
}

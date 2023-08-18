using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.UserServices;
using MyAppRoot.Domain.Entities.Offices;
using MyAppRoot.Domain.Identity;
using MyAppRoot.TestData.Constants;

namespace AppServicesTests.Offices;

public class Create
{
    [Test]
    public async Task WhenResourceIsValid_ReturnsId()
    {
        var item = new Office(Guid.NewGuid(), TestConstants.ValidName);
        var repoMock = Substitute.For<IOfficeRepository>();
        var managerMock = Substitute.For<IOfficeManager>();
        managerMock.CreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(item);
        var userServiceMock = Substitute.For<IUserService>();
        userServiceMock.GetCurrentUserAsync().Returns((ApplicationUser?)null);
        var appService = new OfficeService(repoMock, managerMock,
            AppServicesTestsSetup.Mapper!, userServiceMock);
        var resource = new OfficeCreateDto { Name = TestConstants.ValidName };

        var result = await appService.CreateAsync(resource);

        result.Should().Be(item.Id);
    }
}

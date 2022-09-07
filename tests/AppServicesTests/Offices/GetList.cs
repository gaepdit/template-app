using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.UserServices;
using MyAppRoot.Domain.Entities;
using MyAppRoot.Domain.Offices;
using MyAppRoot.TestData.Constants;

namespace AppServicesTests.Offices;

public class GetList
{
    [Test]
    public async Task WhenItemsExist_ReturnsViewDtoList()
    {
        var office = new Office(Guid.Empty, TestConstants.ValidName);
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Local",
            LastName = "User",
            Email = "local.user@example.net",
            UserName = "local.user@example.net",
            NormalizedUserName = "local.user@example.net".ToUpperInvariant(),
        };
        var itemList = new List<Office> { office };

        var repoMock = new Mock<IOfficeRepository>();
        repoMock.Setup(l => l.GetListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemList);
        var managerMock = new Mock<IOfficeManager>();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(l => l.GetCurrentUserAsync())
            .ReturnsAsync((ApplicationUser?)null);
        var appService = new OfficeAppService(repoMock.Object, managerMock.Object,
            AppServicesTestsGlobal.Mapper!, userServiceMock.Object);

        var result = await appService.GetListAsync();

        result.Should().BeEquivalentTo(itemList);
    }
}

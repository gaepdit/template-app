using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.UserServices;
using MyAppRoot.Domain.Entities.Offices;
using MyAppRoot.Domain.Identity;
using MyAppRoot.TestData.Constants;

namespace AppServicesTests.Offices;

public class GetActiveStaff
{
    [Test]
    public async Task WhenOfficeExists_ReturnsViewDtoList()
    {
        var user = new ApplicationUser
        {
            GivenName = TestConstants.ValidName,
            FamilyName = TestConstants.NewValidName,
            Email = TestConstants.ValidEmail,
        };

        var itemList = new List<ApplicationUser> { user };
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.GetActiveStaffMembersListAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(itemList);
        var managerMock = Substitute.For<IOfficeManager>();
        var userServiceMock = Substitute.For<IUserService>();

        var appService = new OfficeService(repoMock, managerMock,
            AppServicesTestsSetup.Mapper!, userServiceMock);

        var result = await appService.GetActiveStaffAsync(Guid.Empty);

        result.Should().ContainSingle(e =>
            string.Equals(e.GivenName, user.GivenName, StringComparison.Ordinal) &&
            string.Equals(e.FamilyName, user.FamilyName, StringComparison.Ordinal) &&
            string.Equals(e.Email, user.Email, StringComparison.Ordinal));
    }
}

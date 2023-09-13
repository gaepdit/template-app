using MyApp.AppServices.Offices;
using MyApp.TestData.Constants;
using MyApp.WebApp.Api;

namespace WebAppTests.Api;

[TestFixture]
public class OfficeApiTests
{
    [Test]
    public async Task ListOffices_ReturnsListOfOffices()
    {
        List<OfficeViewDto> officeList = new()
            { new OfficeViewDto(Guid.Empty, TextData.ValidName, true) };
        var serviceMock = Substitute.For<IOfficeService>();
        serviceMock.GetListAsync(CancellationToken.None).Returns(officeList);
        var apiController = new OfficeApiController(serviceMock);

        var result = await apiController.ListOfficesAsync();

        result.Should().BeEquivalentTo(officeList);
    }
}

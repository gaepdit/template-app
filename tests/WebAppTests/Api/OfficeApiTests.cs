using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.TestData.Constants;
using MyAppRoot.WebApp.Api;

namespace WebAppTests.Api;

[TestFixture]
public class OfficeApiTests
{
    [Test]
    public async Task ListOffices_ReturnsListOfOffices()
    {
        List<OfficeViewDto> officeList = new()
            { new OfficeViewDto { Id = Guid.Empty, Name = TestConstants.ValidName } };
        var serviceMock = Substitute.For<IOfficeService>();
        serviceMock.GetListAsync(CancellationToken.None).Returns(officeList);
        var apiController = new OfficeApiController(serviceMock);

        var result = await apiController.ListOfficesAsync();

        result.Should().BeEquivalentTo(officeList);
    }

    [Test]
    public async Task GetOffice_ReturnsOfficeView()
    {
        var item = Substitute.For<OfficeViewDto>();
        var serviceMock = Substitute.For<IOfficeService>();
        serviceMock.FindAsync(Guid.Empty, CancellationToken.None).Returns(item);
        var apiController = new OfficeApiController(serviceMock);

        var response = await apiController.GetOfficeAsync(Guid.Empty);

        using (new AssertionScope())
        {
            response.Result.Should().BeOfType<OkObjectResult>();
            var result = response.Result as OkObjectResult;
            result.Should().NotBeNull();
            result?.Value.Should().Be(item);
        }
    }

    [Test]
    public async Task GetOffice_UnknownIdReturnsNotFound()
    {
        var serviceMock = Substitute.For<IOfficeService>();
        serviceMock.FindAsync(Arg.Any<Guid>(), CancellationToken.None).Returns(null as OfficeViewDto);
        var apiController = new OfficeApiController(serviceMock);

        var response = await apiController.GetOfficeAsync(Guid.Empty);

        using (new AssertionScope())
        {
            response.Result.Should().BeOfType<ObjectResult>();
            var result = response.Result as ObjectResult;
            result?.StatusCode.Should().Be(404);
        }
    }
}

using FluentAssertions.Execution;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.TestData.Constants;
using MyAppRoot.WebApp.Models;
using MyAppRoot.WebApp.Pages.Admin.Maintenance.Offices;
using MyAppRoot.WebApp.Platform.RazorHelpers;

namespace WebAppTests.Pages.Admin.Maintenance.Offices;

public class IndexTests
{
    private static readonly List<OfficeViewDto> ListTest = new()
        { new OfficeViewDto { Id = Guid.Empty, Name = TestConstants.ValidName } };

    [Test]
    public async Task OnGet_ReturnsWithList()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        serviceMock.Setup(l => l.GetListAsync(CancellationToken.None))
            .ReturnsAsync(ListTest);
        var page = new IndexModel { TempData = WebAppTestsGlobal.GetPageTempData() };

        await page.OnGetAsync(serviceMock.Object);

        using (new AssertionScope())
        {
            page.Items.Should().BeEquivalentTo(ListTest);
            page.Message.Should().BeNull();
            page.HighlightId.Should().BeNull();
        }
    }

    [Test]
    public async Task SetDisplayMessage_ReturnsWithDisplayMessage()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        serviceMock.Setup(l => l.GetListAsync(CancellationToken.None))
            .ReturnsAsync(ListTest);
        var page = new IndexModel { TempData = WebAppTestsGlobal.GetPageTempData() };
        var expectedMessage = new DisplayMessage(DisplayMessage.AlertContext.Info, "Info message");

        page.TempData.SetDisplayMessage(expectedMessage.Context, expectedMessage.Message);
        await page.OnGetAsync(serviceMock.Object);

        page.Message.Should().BeEquivalentTo(expectedMessage);
    }
}

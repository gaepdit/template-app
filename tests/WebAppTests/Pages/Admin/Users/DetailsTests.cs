using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.Domain.Identity;
using MyAppRoot.TestData.Constants;
using MyAppRoot.WebApp.Pages.Admin.Users;

namespace WebAppTests.Pages.Admin.Users;

public class DetailsTests
{
    [Test]
    public async Task OnGet_PopulatesThePageModel()
    {
        var staffView = new StaffViewDto
        {
            Id = Guid.Empty.ToString(),
            Email = TestConstants.ValidEmail,
            GivenName = TestConstants.ValidName,
            FamilyName = TestConstants.ValidName,
        };
        var serviceMock = new Mock<IStaffAppService>();
        serviceMock.Setup(l => l.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(staffView);
        serviceMock.Setup(l => l.GetAppRolesAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<AppRole>());
        var pageModel = new DetailsModel { TempData = WebAppTestsGlobal.GetPageTempData() };

        var result = await pageModel.OnGetAsync(serviceMock.Object, staffView.Id);

        using (new AssertionScope())
        {
            result.Should().BeOfType<PageResult>();
            pageModel.DisplayStaff.Should().Be(staffView);
            pageModel.Roles.Should().BeEmpty();
            pageModel.Message.Should().BeNull();
        }
    }

    [Test]
    public async Task OnGet_MissingIdReturnsNotFound()
    {
        var serviceMock = new Mock<IStaffAppService>();
        var pageModel = new DetailsModel { TempData = WebAppTestsGlobal.GetPageTempData() };

        var result = await pageModel.OnGetAsync(serviceMock.Object, null);

        using (new AssertionScope())
        {
            result.Should().BeOfType<RedirectToPageResult>();
            ((RedirectToPageResult)result).PageName.Should().Be("Index");
        }
    }

    [Test]
    public async Task OnGet_NonexistentIdReturnsNotFound()
    {
        var serviceMock = new Mock<IStaffAppService>();
        serviceMock.Setup(l => l.FindAsync(It.IsAny<string>()))
            .ReturnsAsync((StaffViewDto?)null);
        var pageModel = new DetailsModel { TempData = WebAppTestsGlobal.GetPageTempData() };

        var result = await pageModel.OnGetAsync(serviceMock.Object, Guid.Empty.ToString());

        result.Should().BeOfType<NotFoundResult>();
    }
}

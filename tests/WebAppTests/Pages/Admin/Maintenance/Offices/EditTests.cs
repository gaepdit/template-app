using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.TestData.Constants;
using MyAppRoot.WebApp.Models;
using MyAppRoot.WebApp.Pages.Admin.Maintenance.Offices;
using MyAppRoot.WebApp.Platform.PageModelHelpers;
using System.Security.Claims;

namespace WebAppTests.Pages.Admin.Maintenance.Offices;

public class EditTests
{
    private static readonly OfficeUpdateDto ItemTest = new() { Id = Guid.Empty, Name = TestConstants.ValidName };

    private static readonly StaffViewDto StaffViewTest = new() { Id = Guid.Empty.ToString(), Active = true };

    [Test]
    public async Task OnGet_GivenSameOffice_ReturnsWithItem()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        serviceMock.Setup(l => l.FindForUpdateAsync(ItemTest.Id, CancellationToken.None)).ReturnsAsync(ItemTest);
        var staffServiceMock = new Mock<IStaffAppService>();
        staffServiceMock.Setup(l => l.GetCurrentUserAsync())
            .ReturnsAsync(StaffViewTest);
        var authorizationMock = new Mock<IAuthorizationService>();
        authorizationMock.Setup(l =>
                l.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object?>(),
                    It.IsAny<IAuthorizationRequirement[]>()))
            .ReturnsAsync(AuthorizationResult.Success);
        var page = new EditModel(serviceMock.Object, Mock.Of<IValidator<OfficeUpdateDto>>(),
                staffServiceMock.Object, authorizationMock.Object)
            { TempData = WebAppTestsGlobal.PageTempData(), PageContext = WebAppTestsGlobal.PageContextWithUser() };

        await page.OnGetAsync(ItemTest.Id);

        using (new AssertionScope())
        {
            page.Item.Should().BeEquivalentTo(ItemTest);
            page.OriginalName.Should().Be(ItemTest.Name);
            page.HighlightId.Should().Be(Guid.Empty);
            page.IsMyOffice.Should().BeTrue();
        }
    }

    [Test]
    public async Task OnGet_GivenDifferentOffice_ReturnsWithItem()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        serviceMock.Setup(l => l.FindForUpdateAsync(ItemTest.Id, CancellationToken.None)).ReturnsAsync(ItemTest);
        var staffServiceMock = new Mock<IStaffAppService>();
        staffServiceMock.Setup(l => l.GetCurrentUserAsync())
            .ReturnsAsync(StaffViewTest);
        var authorizationMock = new Mock<IAuthorizationService>();
        authorizationMock.Setup(l =>
                l.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object?>(),
                    It.IsAny<IAuthorizationRequirement[]>()))
            .ReturnsAsync(AuthorizationResult.Failed);
        var page = new EditModel(serviceMock.Object, Mock.Of<IValidator<OfficeUpdateDto>>(),
                staffServiceMock.Object, authorizationMock.Object)
            { TempData = WebAppTestsGlobal.PageTempData(), PageContext = WebAppTestsGlobal.PageContextWithUser() };

        await page.OnGetAsync(ItemTest.Id);

        using (new AssertionScope())
        {
            page.Item.Should().BeEquivalentTo(ItemTest);
            page.OriginalName.Should().Be(ItemTest.Name);
            page.HighlightId.Should().Be(Guid.Empty);
            page.IsMyOffice.Should().BeFalse();
        }
    }

    [Test]
    public async Task OnGet_GivenNullId_ReturnsNotFound()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        var staffServiceMock = new Mock<IStaffAppService>();
        staffServiceMock.Setup(l => l.GetCurrentUserAsync())
            .ReturnsAsync(StaffViewTest);
        var page = new EditModel(serviceMock.Object, Mock.Of<IValidator<OfficeUpdateDto>>(),
                staffServiceMock.Object, Mock.Of<IAuthorizationService>())
            { TempData = WebAppTestsGlobal.PageTempData() };

        var result = await page.OnGetAsync(null);

        using (new AssertionScope())
        {
            result.Should().BeOfType<RedirectToPageResult>();
            ((RedirectToPageResult)result).PageName.Should().Be("Index");
        }
    }

    [Test]
    public async Task OnGet_GivenInvalidId_ReturnsNotFound()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        serviceMock.Setup(l => l.FindForUpdateAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((OfficeUpdateDto?)null);
        var staffServiceMock = new Mock<IStaffAppService>();
        staffServiceMock.Setup(l => l.GetCurrentUserAsync())
            .ReturnsAsync(StaffViewTest);
        var page = new EditModel(serviceMock.Object, Mock.Of<IValidator<OfficeUpdateDto>>(),
                staffServiceMock.Object, Mock.Of<IAuthorizationService>())
            { TempData = WebAppTestsGlobal.PageTempData() };

        var result = await page.OnGetAsync(Guid.Empty);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task OnPost_GivenSuccess_ReturnsRedirectWithDisplayMessage()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        var validatorMock = new Mock<IValidator<OfficeUpdateDto>>();
        validatorMock.Setup(l => l.ValidateAsync(It.IsAny<OfficeUpdateDto>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());
        var page = new EditModel(serviceMock.Object, validatorMock.Object,
                Mock.Of<IStaffAppService>(), Mock.Of<IAuthorizationService>())
            { Item = ItemTest, TempData = WebAppTestsGlobal.PageTempData() };
        var expectedMessage =
            new DisplayMessage(DisplayMessage.AlertContext.Success, $"“{ItemTest.Name}” successfully updated.");

        var result = await page.OnPostAsync();

        using (new AssertionScope())
        {
            page.HighlightId.Should().Be(ItemTest.Id);
            page.TempData.GetDisplayMessage().Should().BeEquivalentTo(expectedMessage);
            result.Should().BeOfType<RedirectToPageResult>();
            ((RedirectToPageResult)result).PageName.Should().Be("Index");
        }
    }

    [Test]
    public async Task OnPost_GivenInvalidItem_ReturnsPageWithModelErrors()
    {
        var serviceMock = new Mock<IOfficeAppService>();
        var validatorMock = new Mock<IValidator<OfficeUpdateDto>>();
        var validationFailures = new List<ValidationFailure> { new("property", "message") };
        validatorMock.Setup(l => l.ValidateAsync(It.IsAny<OfficeUpdateDto>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult(validationFailures));
        var page = new EditModel(serviceMock.Object, validatorMock.Object,
                Mock.Of<IStaffAppService>(), Mock.Of<IAuthorizationService>())
            { Item = ItemTest, TempData = WebAppTestsGlobal.PageTempData() };

        var result = await page.OnPostAsync();

        using (new AssertionScope())
        {
            result.Should().BeOfType<PageResult>();
            page.ModelState.IsValid.Should().BeFalse();
        }
    }
}

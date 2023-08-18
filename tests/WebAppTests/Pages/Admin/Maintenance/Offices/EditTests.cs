using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.AppServices.Staff.Dto;
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
        var serviceMock = Substitute.For<IOfficeService>();
        serviceMock.FindForUpdateAsync(ItemTest.Id, Arg.Any<CancellationToken>()).Returns(ItemTest);
        var staffServiceMock = Substitute.For<IStaffService>();
        staffServiceMock.GetCurrentUserAsync().Returns(StaffViewTest);
        var authorizationMock = Substitute.For<IAuthorizationService>();
        authorizationMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
            Arg.Any<IAuthorizationRequirement[]>()).Returns(AuthorizationResult.Success());
        var page = new EditModel(serviceMock, Substitute.For<IValidator<OfficeUpdateDto>>(),
                staffServiceMock, authorizationMock)
            { TempData = WebAppTestsSetup.PageTempData(), PageContext = WebAppTestsSetup.PageContextWithUser() };

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
        var serviceMock = Substitute.For<IOfficeService>();
        serviceMock.FindForUpdateAsync(ItemTest.Id, CancellationToken.None).Returns(ItemTest);
        var staffServiceMock = Substitute.For<IStaffService>();
        staffServiceMock.GetCurrentUserAsync().Returns(StaffViewTest);
        var authorizationMock = Substitute.For<IAuthorizationService>();
        authorizationMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
            Arg.Any<IAuthorizationRequirement[]>()).Returns(AuthorizationResult.Failed());
        var page = new EditModel(serviceMock, Substitute.For<IValidator<OfficeUpdateDto>>(),
                staffServiceMock, authorizationMock)
            { TempData = WebAppTestsSetup.PageTempData(), PageContext = WebAppTestsSetup.PageContextWithUser() };

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
        var serviceMock = Substitute.For<IOfficeService>();
        var staffServiceMock = Substitute.For<IStaffService>();
        staffServiceMock.GetCurrentUserAsync().Returns(StaffViewTest);
        var page = new EditModel(serviceMock, Substitute.For<IValidator<OfficeUpdateDto>>(),
                staffServiceMock, Substitute.For<IAuthorizationService>())
            { TempData = WebAppTestsSetup.PageTempData() };

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
        var serviceMock = Substitute.For<IOfficeService>();
        serviceMock.FindForUpdateAsync(Arg.Any<Guid>(), CancellationToken.None).Returns((OfficeUpdateDto?)null);
        var staffServiceMock = Substitute.For<IStaffService>();
        staffServiceMock.GetCurrentUserAsync().Returns(StaffViewTest);
        var page = new EditModel(serviceMock, Substitute.For<IValidator<OfficeUpdateDto>>(),
                staffServiceMock, Substitute.For<IAuthorizationService>())
            { TempData = WebAppTestsSetup.PageTempData() };

        var result = await page.OnGetAsync(Guid.Empty);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task OnPost_GivenSuccess_ReturnsRedirectWithDisplayMessage()
    {
        var serviceMock = Substitute.For<IOfficeService>();
        var validatorMock = Substitute.For<IValidator<OfficeUpdateDto>>();
        validatorMock.ValidateAsync(Arg.Any<OfficeUpdateDto>(), CancellationToken.None).Returns(new ValidationResult());
        var page = new EditModel(serviceMock, validatorMock,
                Substitute.For<IStaffService>(), Substitute.For<IAuthorizationService>())
            { Item = ItemTest, TempData = WebAppTestsSetup.PageTempData() };
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
        var serviceMock = Substitute.For<IOfficeService>();
        var validatorMock = Substitute.For<IValidator<OfficeUpdateDto>>();
        var validationFailures = new List<ValidationFailure> { new("property", "message") };
        validatorMock.ValidateAsync(Arg.Any<OfficeUpdateDto>(), CancellationToken.None)
            .Returns(new ValidationResult(validationFailures));
        var page = new EditModel(serviceMock, validatorMock,
                Substitute.For<IStaffService>(), Substitute.For<IAuthorizationService>())
            { Item = ItemTest, TempData = WebAppTestsSetup.PageTempData() };

        var result = await page.OnPostAsync();

        using (new AssertionScope())
        {
            result.Should().BeOfType<PageResult>();
            page.ModelState.IsValid.Should().BeFalse();
        }
    }
}

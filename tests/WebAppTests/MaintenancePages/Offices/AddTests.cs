using MyApp.AppServices.Offices;
using MyApp.AppServices.Staff;
using MyApp.TestData.Constants;
using MyApp.WebApp.Models;
using MyApp.WebApp.Pages.Admin.Maintenance.Offices;
using MyApp.WebApp.Platform.PageModelHelpers;

namespace WebAppTests.MaintenancePages.Offices;

public class AddTests
{
    private static readonly OfficeCreateDto ItemTest = new(TextData.ValidName);

    [Test]
    public async Task OnPost_GivenSuccess_ReturnsRedirectWithDisplayMessage()
    {
        // Arrange
        var officeServiceMock = Substitute.For<IOfficeService>();
        officeServiceMock.CreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Guid.Empty);

        var validatorMock = Substitute.For<IValidator<OfficeCreateDto>>();
        validatorMock.ValidateAsync(Arg.Any<OfficeCreateDto>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var page = new AddModel(officeServiceMock, validatorMock)
            { Item = ItemTest, TempData = WebAppTestsSetup.PageTempData() };

        var expectedMessage = new DisplayMessage(DisplayMessage.AlertContext.Success,
            $"“{ItemTest.Name}” successfully added.", []);

        // Act
        var result = await page.OnPostAsync();

        // Assert
        using var scope = new AssertionScope();
        page.HighlightId.Should().Be(Guid.Empty);
        page.TempData.GetDisplayMessage().Should().BeEquivalentTo(expectedMessage);
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("Index");
    }

    [Test]
    public async Task OnPost_GivenInvalidItem_ReturnsPageWithModelErrors()
    {
        // Arrange
        var staffServiceMock = Substitute.For<IStaffService>();
        staffServiceMock.GetAsListItemsAsync(Arg.Any<bool>())
            .Returns(new List<ListItem<string>>());

        var validationFailures = new List<ValidationFailure> { new("property", "message") };

        var validatorMock = Substitute.For<IValidator<OfficeCreateDto>>();
        validatorMock.ValidateAsync(Arg.Any<OfficeCreateDto>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));

        var page = new AddModel(Substitute.For<IOfficeService>(), validatorMock)
            { Item = ItemTest, TempData = WebAppTestsSetup.PageTempData() };

        // Act
        var result = await page.OnPostAsync();

        // Assert
        using var scope = new AssertionScope();
        result.Should().BeOfType<PageResult>();
        page.ModelState.IsValid.Should().BeFalse();
    }
}

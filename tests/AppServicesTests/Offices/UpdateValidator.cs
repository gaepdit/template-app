using FluentValidation.TestHelper;
using MyApp.AppServices.Offices;
using MyApp.AppServices.Offices.Validators;
using MyApp.Domain.Entities.Offices;
using MyApp.TestData.Constants;

namespace AppServicesTests.Offices;

public class UpdateValidator
{
    [Test]
    public async Task ValidDto_ReturnsAsValid()
    {
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Office?)null);
        var model = new OfficeUpdateDto(Id: Guid.Empty, Name: TextData.ValidName, Active: true);

        var validator = new OfficeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task DuplicateName_ReturnsAsInvalid()
    {
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Office(Guid.NewGuid(), TextData.ValidName));
        var model = new OfficeUpdateDto(Id: Guid.Empty, Name: TextData.ValidName, Active: true);

        var validator = new OfficeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(e => e.Name)
            .WithErrorMessage("The name entered already exists.");
    }

    [Test]
    public async Task DuplicateName_ForSameId_ReturnsAsValid()
    {
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Office(Guid.Empty, TextData.ValidName));
        var model = new OfficeUpdateDto(Id: Guid.Empty, Name: TextData.ValidName, Active: true);

        var validator = new OfficeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task NameTooShort_ReturnsAsInvalid()
    {
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Office?)null);
        var model = new OfficeUpdateDto(Id: Guid.Empty, Name: TextData.ShortName, Active: true);

        var validator = new OfficeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(e => e.Name);
    }
}

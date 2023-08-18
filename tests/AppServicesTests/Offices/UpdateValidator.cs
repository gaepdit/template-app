using FluentValidation.TestHelper;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Offices.Validators;
using MyAppRoot.Domain.Entities.Offices;
using MyAppRoot.TestData.Constants;

namespace AppServicesTests.Offices;

public class UpdateValidator
{
    [Test]
    public async Task ValidDto_ReturnsAsValid()
    {
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Office?)null);
        var model = new OfficeUpdateDto
        {
            Id = Guid.Empty,
            Name = TestConstants.ValidName,
            Active = true,
        };

        var validator = new OfficeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldNotHaveValidationErrorFor(e => e.Name);
    }

    [Test]
    public async Task DuplicateName_ReturnsAsInvalid()
    {
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Office(Guid.NewGuid(), TestConstants.ValidName));
        var model = new OfficeUpdateDto
        {
            Id = Guid.Empty,
            Name = TestConstants.ValidName,
            Active = true,
        };

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
            .Returns(new Office(Guid.Empty, TestConstants.ValidName));
        var model = new OfficeUpdateDto
        {
            Id = Guid.Empty,
            Name = TestConstants.ValidName,
            Active = true,
        };

        var validator = new OfficeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldNotHaveValidationErrorFor(e => e.Name);
    }

    [Test]
    public async Task NameTooShort_ReturnsAsInvalid()
    {
        var repoMock = Substitute.For<IOfficeRepository>();
        repoMock.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Office?)null);
        var model = new OfficeUpdateDto() { Name = TestConstants.ShortName };

        var validator = new OfficeUpdateValidator(repoMock);
        var result = await validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(e => e.Name);
    }
}

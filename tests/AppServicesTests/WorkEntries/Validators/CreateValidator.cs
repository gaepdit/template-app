using FluentValidation.TestHelper;
using MyApp.AppServices.WorkEntries.CommandDto;
using MyApp.AppServices.WorkEntries.Validators;
using MyApp.TestData.Constants;

namespace AppServicesTests.WorkEntries.Validators;

public class CreateValidator
{
    [Test]
    public async Task ValidDto_ReturnsAsValid()
    {
        var model = new WorkEntryCreateDto
        {
            EntryTypeId = Guid.NewGuid(),
            Notes = TextData.Paragraph,
        };

        var validator = new WorkEntryCreateValidator();

        var result = await validator.TestValidateAsync(model);

        using var scope = new AssertionScope();
        result.ShouldNotHaveValidationErrorFor(dto => dto.EntryTypeId);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Notes);
    }

    [Test]
    public async Task MissingCurrentOffice_ReturnsAsInvalid()
    {
        var model = new WorkEntryCreateDto();
        var validator = new WorkEntryCreateValidator();

        var result = await validator.TestValidateAsync(model);

        using var scope = new AssertionScope();
        result.ShouldHaveValidationErrorFor(dto => dto.EntryTypeId);
        result.ShouldHaveValidationErrorFor(dto => dto.Notes);
    }
}

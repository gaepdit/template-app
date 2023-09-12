using FluentValidation;
using MyApp.AppServices.Customers.Dto;

namespace MyApp.AppServices.Customers.Validators;

public class ContactCreateValidator : AbstractValidator<ContactCreateDto>
{
    public ContactCreateValidator()
    {
        RuleFor(e => e.Email).EmailAddress();

        RuleFor(e => e.Title)
            .Must((c, _) =>
                !string.IsNullOrWhiteSpace(c.GivenName) ||
                !string.IsNullOrWhiteSpace(c.FamilyName) ||
                !string.IsNullOrWhiteSpace(c.Title))
            .WithMessage("At least a name or title must be entered to create a contact.");
    }
}

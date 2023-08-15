using FluentValidation;
using MyAppRoot.AppServices.Staff.Dto;
using MyAppRoot.Domain.Identity;

namespace MyAppRoot.AppServices.Staff.Validators;

public class StaffUpdateValidator : AbstractValidator<StaffUpdateDto>
{
    public StaffUpdateValidator()
    {
        RuleFor(e => e.Phone).MaximumLength(ApplicationUser.MaxPhoneLength);
    }
}

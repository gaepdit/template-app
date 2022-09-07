using MyAppRoot.Domain.Entities;
using FluentValidation;

namespace MyAppRoot.AppServices.StaffServices;

public class StaffUpdateValidator : AbstractValidator<StaffUpdateDto>
{
    public StaffUpdateValidator()
    {
        RuleFor(e => e.Phone).MaximumLength(ApplicationUser.MaxPhoneLength);
    }
}

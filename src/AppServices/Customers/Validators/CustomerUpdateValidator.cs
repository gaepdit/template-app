using FluentValidation;
using MyApp.AppServices.Customers.Dto;
using MyApp.Domain.Entities.Customers;

namespace MyApp.AppServices.Customers.Validators;

public class CustomerUpdateValidator : AbstractValidator<CustomerUpdateDto>
{
    public CustomerUpdateValidator()
    {
        RuleFor(e => e.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(Customer.MinNameLength);
    }
}

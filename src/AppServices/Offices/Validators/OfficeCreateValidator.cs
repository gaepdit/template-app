using FluentValidation;
using MyApp.Domain.Entities.EntityBase;
using MyApp.Domain.Entities.Offices;

namespace MyApp.AppServices.Offices.Validators;

public class OfficeCreateValidator : AbstractValidator<OfficeCreateDto>
{
    private readonly IOfficeRepository _repository;

    public OfficeCreateValidator(IOfficeRepository repository)
    {
        _repository = repository;

        RuleFor(e => e.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(SimpleNamedEntity.MinNameLength, SimpleNamedEntity.MaxNameLength)
            .MustAsync(async (_, name, token) => await NotDuplicateName(name, token))
            .WithMessage("The name entered already exists.");
    }

    private async Task<bool> NotDuplicateName(string name, CancellationToken token = default) =>
        await _repository.FindByNameAsync(name, token) is null;
}

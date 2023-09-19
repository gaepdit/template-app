using MyApp.Domain;
using System.ComponentModel.DataAnnotations;

namespace MyApp.AppServices.DtoBase;

public interface IDtoHasNameProperty
{
    string Name { get; }
}

public abstract record SimpleNamedEntityViewDto
(
    Guid Id,
    string Name,
    bool Active
) : IDtoHasNameProperty;

public abstract record SimpleNamedEntityCreateDto
(
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.MaximumNameLength, MinimumLength = Constants.MinimumNameLength)]
    string Name
);

public abstract record SimpleNamedEntityUpdateDto
(
    Guid Id,
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.MaximumNameLength, MinimumLength = Constants.MinimumNameLength)]
    string Name,
    bool Active
);

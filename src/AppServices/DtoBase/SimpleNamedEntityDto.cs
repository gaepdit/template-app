using GaEpd.AppLibrary.Domain.Entities;
using MyApp.Domain;
using System.ComponentModel.DataAnnotations;

namespace MyApp.AppServices.DtoBase;

public abstract record SimpleNamedEntityViewDto
(
    Guid Id,
    string Name,
    bool Active
) : INamedEntity;

public abstract record SimpleNamedEntityCreateDto
(
    [Required(AllowEmptyStrings = false)]
    [StringLength(AppConstants.MaximumNameLength, MinimumLength = AppConstants.MinimumNameLength)]
    string Name
);

public abstract record SimpleNamedEntityUpdateDto
(
    Guid Id,
    [Required(AllowEmptyStrings = false)]
    [StringLength(AppConstants.MaximumNameLength, MinimumLength = AppConstants.MinimumNameLength)]
    string Name,
    bool Active
);

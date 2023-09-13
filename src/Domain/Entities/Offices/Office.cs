using MyApp.Domain.Entities.EntityBase;
using MyApp.Domain.Identity;

namespace MyApp.Domain.Entities.Offices;

public class Office : SimpleNamedEntity
{
    public Office(Guid id, string name) : base(id, name) { }

    [UsedImplicitly]
    public ICollection<ApplicationUser> StaffMembers { get; set; } = new List<ApplicationUser>();
}

using MyApp.Domain.Entities.Customers;
using MyApp.Domain.Identity;
using MyApp.Domain.ValueObjects;

namespace MyApp.Domain.Entities.Contacts;

public class Contact : AuditableSoftDeleteEntity
{
    // Constructors

    [UsedImplicitly] // Used by ORM.
    private Contact() { }

    internal Contact(Guid id, Customer customer) : base(id)
    {
        Customer = customer;
    }

    // Properties

    public Customer Customer { get; private init; } = default!;
    public ApplicationUser? EnteredBy { get; set; }
    public DateTimeOffset? EnteredOn { get; init; }

    public string? Honorific { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Title { get; set; }

    [EmailAddress]
    [StringLength(150)]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public string? Notes { get; set; }
    public IncompleteAddress Address { get; set; } = default!;
}

﻿using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.ValueObjects;

namespace MyApp.Domain.Entities.Customers;

public class Customer : AuditableSoftDeleteEntity
{
    // Constants

    public const int MinNameLength = 2;

    // Constructors

    [UsedImplicitly] // Used by ORM.
    private Customer() { }

    internal Customer(Guid id) : base(id) { }

    // Properties

    [StringLength(int.MaxValue, MinimumLength = MinNameLength)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? County { get; set; }

    [MaxLength(2000)] // https://stackoverflow.com/q/417142/212978
    public string? Website { get; set; }

    public IncompleteAddress MailingAddress { get; set; } = default!;

    // Collections

    public List<Contact> Contacts { get; set; } = new();

    // Properties: Deletion

    public string? DeleteComments { get; set; }
}

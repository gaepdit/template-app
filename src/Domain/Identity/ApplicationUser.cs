using Microsoft.AspNetCore.Identity;
using MyAppRoot.Domain.Offices;

namespace MyAppRoot.Domain.Identity;

public class ApplicationUser : IdentityUser, IEntity<string>
{
    // IdentityUser includes Id, Email, UserName, and PhoneNumber properties.

    // Properties from external login provider
    [ProtectedPersonalData]
    [StringLength(150)]
    public string GivenName { get; set; } = string.Empty;

    [ProtectedPersonalData]
    [StringLength(150)]
    public string FamilyName { get; set; } = string.Empty;

    // Editable user/staff properties
    public const int MaxPhoneLength = 25;

    [StringLength(MaxPhoneLength)]
    public string? Phone { get; set; }

    [InverseProperty("StaffMembers")]
    public Office? Office { get; set; }

    public bool Active { get; set; } = true;
}

using Microsoft.AspNetCore.Identity;
using MyAppRoot.Domain.Offices;

namespace MyAppRoot.Domain.Identity;

public class ApplicationUser : IdentityUser, IEntity<string>
{
    // IdentityUser includes Id, Email, and UserName properties.

    // Properties from external login provider
   
    /// <summary>
    /// A claim that specifies the given name of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname
    /// </summary>
    [ProtectedPersonalData]
    [StringLength(150)]
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// A claim that specifies the surname of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname
    /// </summary>
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

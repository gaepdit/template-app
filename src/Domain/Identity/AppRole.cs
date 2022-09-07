using GaEpd.Library.ListItems;

namespace MyAppRoot.Domain.Identity;

/// <summary>
/// Authorization Roles for the application.
/// </summary>
public class AppRole
{
    public string Name { get; }
    public string DisplayName { get; }
    public string Description { get; }

    private AppRole(string name, string displayName, string description)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        AllRoles.Add(name, this);
    }

    /// <summary>
    /// A Dictionary of all roles used by the app. The Dictionary key is a string containing 
    /// the <see cref="Microsoft.AspNetCore.Identity.IdentityRole.Name"/> of the role.
    /// (This declaration must appear before the list of static instance types.)
    /// </summary>
    public static Dictionary<string, AppRole> AllRoles { get; } = new();

    public static IEnumerable<ListItem<string>> AllRolesList() =>
        AllRoles.Select(r => new ListItem<string>(r.Key, r.Value.DisplayName));

    // Roles
    // These are the strings that are stored in the database. Avoid modifying these once set!
    public const string Manager = nameof(Manager);
    public const string UserAdmin = nameof(UserAdmin);

    // These static Role objects are used for displaying role information in the UI.

    public static AppRole ManagerRole { get; } = new(
        Manager, "Manager",
        "Can manage things."
    );

    public static AppRole UserAdminRole { get; } = new(
        UserAdmin, "User Account Admin",
        "Can register and edit all users and roles."
    );
}

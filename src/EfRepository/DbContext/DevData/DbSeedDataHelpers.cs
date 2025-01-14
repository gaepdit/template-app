using MyApp.TestData;
using MyApp.TestData.Identity;

namespace MyApp.EfRepository.DbContext.DevData;

public static class DbSeedDataHelpers
{
    public static void SeedAllData(AppDbContext context)
    {
        SeedOfficeData(context);
        SeedIdentityData(context);
        SeedEntryTypeData(context);
        SeedWorkEntryData(context);
    }

    internal static void SeedEntryTypeData(AppDbContext context)
    {
        if (context.EntryTypes.Any()) return;
        context.EntryTypes.AddRange(EntryTypeData.GetData);
        context.SaveChanges();
    }

    private static void SeedWorkEntryData(AppDbContext context)
    {
        if (context.WorkEntries.Any()) return;

        context.Database.BeginTransaction();

        context.WorkEntries.AddRange(WorkEntryData.GetData);
        context.SaveChanges();

        if (!context.EntryActions.Any())
        {
            context.EntryActions.AddRange(EntryActionData.GetData);
            context.SaveChanges();
        }

        context.Database.CommitTransaction();
    }

    internal static void SeedOfficeData(AppDbContext context)
    {
        if (context.Offices.Any()) return;
        context.Offices.AddRange(OfficeData.GetData);
        context.SaveChanges();
    }

    public static void SeedIdentityData(AppDbContext context)
    {
        // Seed Users
        var users = UserData.GetUsers.ToList();
        if (!context.Users.Any()) context.Users.AddRange(users);

        // Seed Roles
        var roles = UserData.GetRoles.ToList();
        if (!context.Roles.Any()) context.Roles.AddRange(roles);

        context.SaveChanges();
    }
}

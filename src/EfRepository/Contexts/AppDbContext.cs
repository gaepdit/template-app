using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.Entities.Customers;
using MyApp.Domain.Entities.Offices;
using MyApp.Domain.Identity;

namespace MyApp.EfRepository.Contexts;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Add domain entities here.
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Office> Offices => Set<Office>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Some properties should always be included.
        // See https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager#model-configuration-for-auto-including-navigations
        builder.Entity<ApplicationUser>().Navigation(user => user.Office).AutoInclude();
        builder.Entity<Contact>().Navigation(contact => contact.Customer).AutoInclude();

#pragma warning disable S125
        // Let's save enums in the database as strings.
        // See https://stackoverflow.com/a/55260541/212978
        // builder.Entity<EntityWithEnum>().Property(d => d.MyEnum).HasConversion(new EnumToStringConverter<MyEnumType>());
#pragma warning restore S125

        // ## The following configurations are Sqlite only. ##
        if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite") return;

#pragma warning disable S125
        // Sqlite and EF Core are in conflict on how to handle collections of owned types.
        // See: https://stackoverflow.com/a/69826156/212978
        // and: https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities#collections-of-owned-types
        // builder.Entity<EntityWithOwnedType>().OwnsMany(e => e.MyOwnedTypeProperty, b => b.HasKey("Id"));
#pragma warning restore S125

        // "Handling DateTimeOffset in SQLite with Entity Framework Core"
        // https://blog.dangl.me/archive/handling-datetimeoffset-in-sqlite-with-entity-framework-core/
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var dateTimeOffsetProperties = entityType.ClrType.GetProperties()
                .Where(info =>
                    info.PropertyType == typeof(DateTimeOffset) || info.PropertyType == typeof(DateTimeOffset?));
            foreach (var property in dateTimeOffsetProperties)
                builder.Entity(entityType.Name).Property(property.Name)
                    .HasConversion(new DateTimeOffsetToBinaryConverter());
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }
}

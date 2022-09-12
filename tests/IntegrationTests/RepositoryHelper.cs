using Microsoft.EntityFrameworkCore;
using MyAppRoot.Domain.Offices;
using MyAppRoot.Infrastructure.Contexts;
using MyAppRoot.Infrastructure.Repositories;
using MyAppRoot.TestData.Offices;
using MyAppRoot.TestData.SeedData;
using TestSupport.EfHelpers;

namespace IntegrationTests;

public sealed class RepositoryHelper : IDisposable
{
    public AppDbContext DbContext { get; set; } = null!;

    private readonly DbContextOptions<AppDbContext> _options = SqliteInMemory.CreateOptions<AppDbContext>();
    private readonly AppDbContext _context;

    private RepositoryHelper()
    {
        _context = new AppDbContext(_options);
        _context.Database.EnsureCreated();
    }

    public static RepositoryHelper CreateRepositoryHelper() => new();

    public void ClearChangeTracker() => _context.ChangeTracker.Clear();

    public IOfficeRepository GetOfficeRepository()
    {
        DbSeedDataHelpers.SeedOfficeData(_context);
        DbContext = new AppDbContext(_options);
        return new OfficeRepository(DbContext);
    }

    public void Dispose() => _context.Dispose();
}

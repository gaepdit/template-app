using MyAppRoot.Infrastructure.Contexts;
using MyAppRoot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using MyAppRoot.TestData.Offices;
using MyAppRoot.Domain.Offices;

namespace IntegrationTests
{
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

        private void SeedOfficeData()
        {
            if (_context.Offices.Any()) return;
            _context.Offices.AddRange(OfficeData.GetOffices);
            _context.SaveChanges();
        }

        public IOfficeRepository GetOfficeRepository()
        {
            SeedOfficeData();
            DbContext = new AppDbContext(_options);
            return new OfficeRepository(DbContext);
        }

        public void Dispose() => _context.Dispose();
    }
}

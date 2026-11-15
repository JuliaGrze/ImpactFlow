using ImpactFlow.Core.Atlas;
using Microsoft.EntityFrameworkCore;

namespace ImpactFlow.Infrastructure.Persistence
{
    public sealed class ImpactFlowDbContext : DbContext
    {
        public ImpactFlowDbContext(DbContextOptions<ImpactFlowDbContext> options) : base(options)
        {
        }

        public DbSet<AtlasMap> AtlasMaps => Set<AtlasMap>();
        public DbSet<AtlasVersion> AtlasVersions => Set<AtlasVersion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ImpactFlowDbContext).Assembly);
        }
    }
}
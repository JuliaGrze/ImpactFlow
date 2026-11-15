using ImpactFlow.Core.Atlas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImpactFlow.Infrastructure.Persistence.Atlas
{
    internal sealed class AtlasMapConfiguration : IEntityTypeConfiguration<AtlasMap>
    {
        public void Configure(EntityTypeBuilder<AtlasMap> builder)
        {
            builder.ToTable("atlas_maps");
            builder.HasKey(map => map.Id);
            builder.Property(map => map.Id).HasColumnName("id").ValueGeneratedNever();
            builder.Property(map => map.Name).HasColumnName("name").IsRequired();
        }
    }
}

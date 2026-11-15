using ImpactFlow.Core.Atlas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImpactFlow.Infrastructure.Persistence.Atlas
{
    internal sealed class AtlasVersionConfiguration : IEntityTypeConfiguration<AtlasVersion>
    {
        public void Configure(EntityTypeBuilder<AtlasVersion> builder)
        {
            builder.ToTable("atlas_versions");
            builder.HasKey(version => version.Id);
            builder.Property(version => version.Id).HasColumnName("id").ValueGeneratedNever();
            builder.Property(version => version.MapId).HasColumnName("map_id");
            builder.Property(version => version.Number).HasColumnName("number");
            builder.Property(version => version.Status).HasColumnName("status");

            builder.HasOne<AtlasMap>().WithMany().HasForeignKey(version => version.MapId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(version => new { version.MapId, version.Number }).IsUnique();

            builder.Ignore(version => version.Nodes);
            builder.Ignore(version => version.Edges);
        }
    }
}

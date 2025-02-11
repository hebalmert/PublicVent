using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.Entities;

namespace Vent.AccessData.ModelConfig.EntitiesConfig;

public class SoftPlanConfig : IEntityTypeConfiguration<SoftPlan>
{
    public void Configure(EntityTypeBuilder<SoftPlan> builder)
    {
        builder.HasKey(e => e.SoftPlanId);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(e => e.Price).HasPrecision(18, 2);
    }
}
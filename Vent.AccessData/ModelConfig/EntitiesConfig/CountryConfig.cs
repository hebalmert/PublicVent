using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.Entities;

namespace Vent.AccessData.ModelConfig.EntitiesConfig;

public class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(e => e.CountryId);
        builder.HasIndex(e => e.Name).IsUnique();
    }
}
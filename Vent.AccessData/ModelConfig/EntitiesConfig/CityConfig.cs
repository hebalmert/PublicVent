using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.Entities;

namespace Vent.AccessData.ModelConfig.EntitiesConfig;

public class CityConfig : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(e => e.CityId);
        builder.HasIndex(e => new { e.StateId, e.Name }).IsUnique();
        //Proteccion de Cascada
        builder.HasOne(e => e.State).WithMany(c => c.Cities).OnDelete(DeleteBehavior.Restrict);
    }
}
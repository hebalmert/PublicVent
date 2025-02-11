using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.Entities;

namespace Vent.AccessData.ModelConfig.EntitiesConfig;

public class StateConfig : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.HasKey(e => e.StateId);
        builder.HasIndex(e => new { e.CountryId, e.Name }).IsUnique();
        //Proteccion de Cascada
        builder.HasOne(e => e.Country).WithMany(c => c.States).OnDelete(DeleteBehavior.Restrict);
    }
}
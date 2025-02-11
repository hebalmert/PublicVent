using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.AccessData.ModelConfig.EntitiesSoftSecConfig;

public class UsuarioRoleConfig : IEntityTypeConfiguration<UsuarioRole>
{
    public void Configure(EntityTypeBuilder<UsuarioRole> builder)
    {
        builder.HasKey(e => e.UsuarioRoleId);
        builder.HasIndex(x => new { x.UsuarioId, x.UserType }).IsUnique();
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Corporation).WithMany(c => c.UsuarioRoles).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Usuario).WithMany(c => c.UsuarioRoles).OnDelete(DeleteBehavior.Restrict);
    }
}
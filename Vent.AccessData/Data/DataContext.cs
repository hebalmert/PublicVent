using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.AccessData.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //Manejo de UserRoles por Usuario
    public DbSet<UserRoleDetails> UserRoleDetails => Set<UserRoleDetails>();

    //Entities
    public DbSet<Country> Countries => Set<Country>();

    public DbSet<State> States => Set<State>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<SoftPlan> SoftPlans => Set<SoftPlan>();
    public DbSet<Corporation> Corporations => Set<Corporation>();
    public DbSet<Manager> Managers => Set<Manager>();

    //EntitiesSoftSec
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<UsuarioRole> UsuarioRoles => Set<UsuarioRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Para tomar los calores de ConfigEntities
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
using BuildingHub.Modules.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Persistence;

public class CommunityDbContext : DbContext
{
    public CommunityDbContext(DbContextOptions<CommunityDbContext> options)
        : base(options)
    {
    }

    public DbSet<BuildingHub.Modules.Community.Domain.Entities.Community> Communities => Set<BuildingHub.Modules.Community.Domain.Entities.Community>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<UnitPerson> UnitPersons => Set<UnitPerson>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CommunityModelConfiguration());
        modelBuilder.ApplyConfiguration(new BuildingConfiguration());
        modelBuilder.ApplyConfiguration(new FloorConfiguration());
        modelBuilder.ApplyConfiguration(new UnitConfiguration());
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new UnitPersonConfiguration());
    }
}

using BuildingHub.Modules.Reference.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Reference.Infrastructure.Persistence;

public sealed class ReferenceDbContext : DbContext
{
    public ReferenceDbContext(
        DbContextOptions<ReferenceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("reference");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ReferenceDbContext).Assembly);
    }
}

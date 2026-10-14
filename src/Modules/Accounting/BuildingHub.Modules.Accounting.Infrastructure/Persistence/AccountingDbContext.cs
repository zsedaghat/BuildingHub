using BuildingHub.Modules.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public class AccountingDbContext : DbContext
{
    public AccountingDbContext(
        DbContextOptions<AccountingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Charge> Charges => Set<Charge>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new ChargeConfiguration());
    }
}
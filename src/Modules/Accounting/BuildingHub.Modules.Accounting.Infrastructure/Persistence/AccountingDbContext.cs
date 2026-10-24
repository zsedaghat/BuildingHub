
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
    public DbSet<UnitCharge> UnitCharges => Set<UnitCharge>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new ChargeConfiguration());

        modelBuilder.ApplyConfiguration(
            new UnitChargeConfiguration());

        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }
}

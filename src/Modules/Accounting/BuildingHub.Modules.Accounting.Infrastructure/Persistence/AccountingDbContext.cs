
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

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<ChargeExpense> ChargeExpenses => Set<ChargeExpense>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new ChargeConfiguration());

        modelBuilder.ApplyConfiguration(
            new UnitChargeConfiguration());

        modelBuilder.ApplyConfiguration(
            new PaymentConfiguration());

        modelBuilder.ApplyConfiguration(
    new ExpenseConfiguration());

        modelBuilder.ApplyConfiguration(
    new ChargeExpenseConfiguration());
    }
}

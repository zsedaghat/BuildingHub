
using BuildingHub.Modules.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public class ChargeExpenseConfiguration
    : IEntityTypeConfiguration<ChargeExpense>
{
    public void Configure(EntityTypeBuilder<ChargeExpense> builder)
    {
        builder.ToTable("ChargeExpenses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExpenseId)
            .IsRequired();

        builder.Property(x => x.ChargeId)
            .IsRequired();

        builder.Property(x => x.AllocatedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<Expense>()
            .WithMany()
            .HasForeignKey(x => x.ExpenseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Charge>()
            .WithMany()
            .HasForeignKey(x => x.ChargeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ExpenseId);

        builder.HasIndex(x => x.ChargeId);

        builder.HasIndex(x => new
        {
            x.ExpenseId,
            x.ChargeId
        }).IsUnique();
    }
}

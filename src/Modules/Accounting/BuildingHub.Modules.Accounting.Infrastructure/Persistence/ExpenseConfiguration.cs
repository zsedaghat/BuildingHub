
using BuildingHub.Modules.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BuildingId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.ExpenseDate)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.BuildingId);
    }
}

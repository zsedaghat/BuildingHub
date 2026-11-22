
using BuildingHub.Modules.Reference.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Reference.Infrastructure.Persistence.Configurations;

public sealed class CurrencyConfiguration
    : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currency", "reference", table =>
        {
            table.HasCheckConstraint(
                "ck_currency_code_uppercase",
                "code ~ '^[A-Z]{3}$'");

            table.HasCheckConstraint(
                "ck_currency_minor_unit_digits",
                "minor_unit_digits BETWEEN 0 AND 6");
        });

        // Primary Key
        builder.HasKey(x => x.Code)
            .HasName("pk_currency");

        // Alternate Key - Required for composite Foreign Keys
        builder.HasAlternateKey(x => new
        {
            x.Code,
            x.MinorUnitDigits
        }).HasName("ak_currency_code_minor_unit_digits");

        // Code
        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasColumnType("character(3)")
            .HasMaxLength(3)
            .IsFixedLength()
            .IsRequired();

        // Name
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        // Minor Unit Digits
        builder.Property(x => x.MinorUnitDigits)
            .HasColumnName("minor_unit_digits")
            .HasColumnType("smallint")
            .IsRequired();

        // Is Active
        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        // Seed Data
        builder.HasData(
            new Currency("IRR", "Iranian Rial", 0),
            new Currency("USD", "US Dollar", 2),
            new Currency("EUR", "Euro", 2),
            new Currency("KWD", "Kuwaiti Dinar", 3)
        );
    }
}

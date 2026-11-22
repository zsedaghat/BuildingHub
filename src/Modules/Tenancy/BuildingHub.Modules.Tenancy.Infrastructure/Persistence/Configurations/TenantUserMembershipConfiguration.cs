
using BuildingHub.Modules.Tenancy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Persistence.Configurations;

public sealed class TenantUserMembershipConfiguration
    : IEntityTypeConfiguration<TenantUserMembership>
{
    public void Configure(
        EntityTypeBuilder<TenantUserMembership> builder)
    {
        builder.ToTable(
            "tenant_user_membership",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_tenant_user_membership_role",
                    "role IN ('TenantAdmin', 'CommunityManager', 'Accountant')");

                table.HasCheckConstraint(
                    "ck_tenant_user_membership_status",
                    "status IN ('Active', 'Suspended')");

                table.HasCheckConstraint(
                    "ck_tenant_user_membership_scope_mode",
                    "scope_mode IN ('All', 'Selected')");

                table.HasCheckConstraint(
                    "ck_tenant_user_membership_dates",
                    "end_at_utc IS NULL OR end_at_utc > start_at_utc");
            });


        builder.HasKey(x => x.Id)
            .HasName("pk_tenant_user_membership");

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ScopeMode)
            .HasColumnName("scope_mode")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.StartAtUtc)
            .HasColumnName("start_at_utc")
            .IsRequired();

        builder.Property(x => x.EndAtUtc)
            .HasColumnName("end_at_utc");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired();

        // Physical FK to Tenant
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName(
                "fk_tenant_user_membership_tenant");

        // Only one non-ended membership per tenant/user
        builder.HasIndex(x => new
        {
            x.TenantId,
            x.UserId
        })
            .IsUnique()
            .HasFilter("end_at_utc IS NULL")
            .HasDatabaseName(
                "ux_tenant_user_membership_active_tenant_user");
    }
}

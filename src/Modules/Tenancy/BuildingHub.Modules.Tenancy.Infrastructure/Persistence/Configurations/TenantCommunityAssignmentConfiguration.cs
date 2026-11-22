
using BuildingHub.Modules.Tenancy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Persistence.Configurations;

public sealed class TenantCommunityAssignmentConfiguration
    : IEntityTypeConfiguration<TenantCommunityAssignment>
{
    public void Configure(
        EntityTypeBuilder<TenantCommunityAssignment> builder)
    {
        builder.ToTable(
            "tenant_community_assignment",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_tenant_community_assignment_dates",
                    "end_at_utc IS NULL OR end_at_utc > start_at_utc");
            });

        builder.HasKey(x => x.Id)
            .HasName("pk_tenant_community_assignment");

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.CommunityId)
            .HasColumnName("community_id")
            .IsRequired();

        builder.Property(x => x.StartAtUtc)
            .HasColumnName("start_at_utc")
            .IsRequired();

        builder.Property(x => x.EndAtUtc)
            .HasColumnName("end_at_utc");

        builder.Property(x => x.TransferReason)
            .HasColumnName("transfer_reason")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

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
                "fk_tenant_community_assignment_tenant");

        // Index for current assignments of a tenant
        builder.HasIndex(x => x.TenantId)
            .HasFilter("end_at_utc IS NULL")
            .HasDatabaseName(
                "ix_tenant_community_assignment_active_tenant");
    }
}

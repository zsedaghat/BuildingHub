
using BuildingHub.Modules.Tenancy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Persistence.Configurations;

public sealed class TenantMembershipScopeConfiguration
    : IEntityTypeConfiguration<TenantMembershipScope>
{
    public void Configure(
        EntityTypeBuilder<TenantMembershipScope> builder)
    {
        builder.ToTable(
            "tenant_membership_scope",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_tenant_membership_scope_type",
                    "scope_type IN ('Community', 'Building')");

                table.HasCheckConstraint(
                    "ck_tenant_membership_scope_ids",
                    "(scope_type = 'Community' AND scope_id = community_id) OR scope_type = 'Building'");
            });

        builder.HasKey(x => x.Id)
            .HasName("pk_tenant_membership_scope");

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.MembershipId)
            .HasColumnName("membership_id")
            .IsRequired();

        builder.Property(x => x.ScopeType)
            .HasColumnName("scope_type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ScopeId)
            .HasColumnName("scope_id")
            .IsRequired();

        builder.Property(x => x.CommunityId)
            .HasColumnName("community_id")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.RevokedAtUtc)
            .HasColumnName("revoked_at_utc");

        builder.HasOne<TenantUserMembership>()
            .WithMany()
            .HasForeignKey(x => x.MembershipId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName(
                "fk_tenant_membership_scope_membership");

        builder.HasIndex(x => new
        {
            x.MembershipId,
            x.ScopeType,
            x.ScopeId
        })
            .IsUnique()
            .HasFilter("revoked_at_utc IS NULL")
            .HasDatabaseName(
                "ux_tenant_membership_scope_active");
    }
}

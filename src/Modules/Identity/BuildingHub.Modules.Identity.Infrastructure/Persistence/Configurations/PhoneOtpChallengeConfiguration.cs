
using BuildingHub.Modules.Identity.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingHub.Modules.Identity.Infrastructure.Persistence.Configurations;

public sealed class PhoneOtpChallengeConfiguration
    : IEntityTypeConfiguration<PhoneOtpChallenge>
{
    public void Configure(
        EntityTypeBuilder<PhoneOtpChallenge> builder)
    {
        builder.ToTable("phone_otp_challenge", "identity", table =>
        {
            table.HasCheckConstraint(
                "ck_phone_otp_attempts",
                "attempt_count >= 0 AND max_attempts > 0 " +
                "AND attempt_count <= max_attempts");

            table.HasCheckConstraint(
                "ck_phone_otp_expiration",
                "expires_at_utc > created_at_utc");

            table.HasCheckConstraint(
                "ck_phone_otp_consumed",
                "(status = 'Consumed' AND consumed_at_utc IS NOT NULL) " +
                "OR (status <> 'Consumed' AND consumed_at_utc IS NULL)");

            table.HasCheckConstraint(
                "ck_phone_otp_status",
                "status IN ('Active', 'Consumed', 'Superseded', " +
                "'Expired', 'Locked')");

            table.HasCheckConstraint(
                "ck_phone_otp_purpose",
                "purpose IN ('Login', 'ConfirmPhone', 'ChangePhone')");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PhoneE164)
            .HasColumnName("phone_e164")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Purpose)
            .HasColumnName("purpose")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CodeHmac)
            .HasColumnName("code_hmac")
            .HasColumnType("bytea")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AttemptCount)
            .HasColumnName("attempt_count")
            .IsRequired();

        builder.Property(x => x.MaxAttempts)
            .HasColumnName("max_attempts")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.ResendAvailableAtUtc)
            .HasColumnName("resend_available_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.ConsumedAtUtc)
            .HasColumnName("consumed_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(x => new
        {
            x.PhoneE164,
            x.Purpose
        })
        .IsUnique()
        .HasDatabaseName("ux_phone_otp_active")
        .HasFilter("status = 'Active'");

        builder.HasIndex(x => new
        {
            x.PhoneE164,
            x.CreatedAtUtc
        })
        .HasDatabaseName("ix_phone_otp_phone_created");
    }
}

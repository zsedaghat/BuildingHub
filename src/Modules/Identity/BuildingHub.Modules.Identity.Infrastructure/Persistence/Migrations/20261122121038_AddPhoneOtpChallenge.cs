using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneOtpChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "phone_otp_challenge",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_e164 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    purpose = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    code_hmac = table.Column<byte[]>(type: "bytea", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    max_attempts = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resend_available_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consumed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone_otp_challenge", x => x.id);
                    table.CheckConstraint("ck_phone_otp_attempts", "attempt_count >= 0 AND max_attempts > 0 AND attempt_count <= max_attempts");
                    table.CheckConstraint("ck_phone_otp_consumed", "(status = 'Consumed' AND consumed_at_utc IS NOT NULL) OR (status <> 'Consumed' AND consumed_at_utc IS NULL)");
                    table.CheckConstraint("ck_phone_otp_expiration", "expires_at_utc > created_at_utc");
                    table.CheckConstraint("ck_phone_otp_purpose", "purpose IN ('Login', 'ConfirmPhone', 'ChangePhone')");
                    table.CheckConstraint("ck_phone_otp_status", "status IN ('Active', 'Consumed', 'Superseded', 'Expired', 'Locked')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_phone_otp_phone_created",
                schema: "identity",
                table: "phone_otp_challenge",
                columns: new[] { "phone_e164", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ux_phone_otp_active",
                schema: "identity",
                table: "phone_otp_challenge",
                columns: new[] { "phone_e164", "purpose" },
                unique: true,
                filter: "status = 'Active'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "phone_otp_challenge",
                schema: "identity");
        }
    }
}

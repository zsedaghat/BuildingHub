using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Tenancy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenancy");

            migrationBuilder.CreateTable(
                name: "tenant",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant", x => x.id);
                    table.CheckConstraint("ck_tenant_status", "status IN ('Active', 'Suspended', 'Closed')");
                });

            migrationBuilder.CreateTable(
                name: "tenant_community_assignment",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    community_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    transfer_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_community_assignment", x => x.id);
                    table.CheckConstraint("ck_tenant_community_assignment_dates", "end_at_utc IS NULL OR end_at_utc > start_at_utc");
                    table.ForeignKey(
                        name: "fk_tenant_community_assignment_tenant",
                        column: x => x.tenant_id,
                        principalSchema: "tenancy",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_user_membership",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    scope_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    start_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_user_membership", x => x.id);
                    table.CheckConstraint("ck_tenant_user_membership_dates", "end_at_utc IS NULL OR end_at_utc > start_at_utc");
                    table.CheckConstraint("ck_tenant_user_membership_role", "role IN ('TenantAdmin', 'CommunityManager', 'Accountant')");
                    table.CheckConstraint("ck_tenant_user_membership_scope_mode", "scope_mode IN ('All', 'Selected')");
                    table.CheckConstraint("ck_tenant_user_membership_status", "status IN ('Active', 'Suspended')");
                    table.ForeignKey(
                        name: "fk_tenant_user_membership_tenant",
                        column: x => x.tenant_id,
                        principalSchema: "tenancy",
                        principalTable: "tenant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_membership_scope",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    scope_id = table.Column<Guid>(type: "uuid", nullable: false),
                    community_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_membership_scope", x => x.id);
                    table.CheckConstraint("ck_tenant_membership_scope_ids", "(scope_type = 'Community' AND scope_id = community_id) OR scope_type = 'Building'");
                    table.CheckConstraint("ck_tenant_membership_scope_type", "scope_type IN ('Community', 'Building')");
                    table.ForeignKey(
                        name: "fk_tenant_membership_scope_membership",
                        column: x => x.membership_id,
                        principalSchema: "tenancy",
                        principalTable: "tenant_user_membership",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_community_assignment_active_tenant",
                schema: "tenancy",
                table: "tenant_community_assignment",
                column: "tenant_id",
                filter: "end_at_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_tenant_membership_scope_active",
                schema: "tenancy",
                table: "tenant_membership_scope",
                columns: new[] { "membership_id", "scope_type", "scope_id" },
                unique: true,
                filter: "revoked_at_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_tenant_user_membership_active_tenant_user",
                schema: "tenancy",
                table: "tenant_user_membership",
                columns: new[] { "tenant_id", "user_id" },
                unique: true,
                filter: "end_at_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenant_community_assignment",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "tenant_membership_scope",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "tenant_user_membership",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "tenant",
                schema: "tenancy");
        }
    }
}

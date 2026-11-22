using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Tenancy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignTenantWithV314 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantType",
                table: "Tenants");

            migrationBuilder.EnsureSchema(
                name: "tenancy");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "tenant",
                newSchema: "tenancy");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "tenancy",
                table: "tenant",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "tenancy",
                table: "tenant",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "tenancy",
                table: "tenant",
                newName: "created_at_utc");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "tenancy",
                table: "tenant",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at_utc",
                schema: "tenancy",
                table: "tenant",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant",
                schema: "tenancy",
                table: "tenant",
                column: "id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_tenant_status",
                schema: "tenancy",
                table: "tenant",
                sql: "status IN ('Active', 'Suspended', 'Closed')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant",
                schema: "tenancy",
                table: "tenant");

            migrationBuilder.DropCheckConstraint(
                name: "ck_tenant_status",
                schema: "tenancy",
                table: "tenant");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "tenancy",
                table: "tenant");

            migrationBuilder.DropColumn(
                name: "updated_at_utc",
                schema: "tenancy",
                table: "tenant");

            migrationBuilder.RenameTable(
                name: "tenant",
                schema: "tenancy",
                newName: "Tenants");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Tenants",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Tenants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "Tenants",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantType",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants",
                column: "Id");
        }
    }
}

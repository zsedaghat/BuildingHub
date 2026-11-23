using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Community.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "community");

            migrationBuilder.CreateTable(
                name: "community",
                schema: "community",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person",
                schema: "community",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    national_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "building",
                schema: "community",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    community_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_building", x => x.id);
                    table.ForeignKey(
                        name: "fk_building_community",
                        column: x => x.community_id,
                        principalSchema: "community",
                        principalTable: "community",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "floor",
                schema: "community",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    building_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    floor_number = table.Column<int>(type: "integer", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_floor", x => x.id);
                    table.UniqueConstraint("ak_floor_id_building_id", x => new { x.id, x.building_id });
                    table.ForeignKey(
                        name: "fk_floor_building",
                        column: x => x.building_id,
                        principalSchema: "community",
                        principalTable: "building",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "unit",
                schema: "community",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    building_id = table.Column<Guid>(type: "uuid", nullable: false),
                    floor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unit_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    area = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit", x => x.id);
                    table.ForeignKey(
                        name: "fk_unit_building",
                        column: x => x.building_id,
                        principalSchema: "community",
                        principalTable: "building",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_unit_floor_building",
                        columns: x => new { x.floor_id, x.building_id },
                        principalSchema: "community",
                        principalTable: "floor",
                        principalColumns: new[] { "id", "building_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "unit_person",
                schema: "community",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit_person", x => x.id);
                    table.CheckConstraint("ck_unit_person_dates", "end_date IS NULL OR start_date IS NULL OR end_date >= start_date");
                    table.ForeignKey(
                        name: "fk_unit_person_person",
                        column: x => x.person_id,
                        principalSchema: "community",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_unit_person_unit",
                        column: x => x.unit_id,
                        principalSchema: "community",
                        principalTable: "unit",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_building_community_name",
                schema: "community",
                table: "building",
                columns: new[] { "community_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_floor_building_floor_number",
                schema: "community",
                table: "floor",
                columns: new[] { "building_id", "floor_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_person_mobile",
                schema: "community",
                table: "person",
                column: "mobile",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_unit_floor_id_building_id",
                schema: "community",
                table: "unit",
                columns: new[] { "floor_id", "building_id" });

            migrationBuilder.CreateIndex(
                name: "ux_unit_building_number",
                schema: "community",
                table: "unit",
                columns: new[] { "building_id", "unit_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_unit_person_person_id",
                schema: "community",
                table: "unit_person",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ux_unit_person_role",
                schema: "community",
                table: "unit_person",
                columns: new[] { "unit_id", "person_id", "role" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unit_person",
                schema: "community");

            migrationBuilder.DropTable(
                name: "person",
                schema: "community");

            migrationBuilder.DropTable(
                name: "unit",
                schema: "community");

            migrationBuilder.DropTable(
                name: "floor",
                schema: "community");

            migrationBuilder.DropTable(
                name: "building",
                schema: "community");

            migrationBuilder.DropTable(
                name: "community",
                schema: "community");
        }
    }
}

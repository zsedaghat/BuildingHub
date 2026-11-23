using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BuildingHub.Modules.Reference.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reference");

            migrationBuilder.CreateTable(
                name: "currency",
                schema: "reference",
                columns: table => new
                {
                    code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    minor_unit_digits = table.Column<byte>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currency", x => x.code);
                    table.UniqueConstraint("ak_currency_code_minor_unit_digits", x => new { x.code, x.minor_unit_digits });
                    table.CheckConstraint("ck_currency_code_uppercase", "code ~ '^[A-Z]{3}$'");
                    table.CheckConstraint("ck_currency_minor_unit_digits", "minor_unit_digits BETWEEN 0 AND 6");
                });

            migrationBuilder.InsertData(
                schema: "reference",
                table: "currency",
                columns: new[] { "code", "is_active", "minor_unit_digits", "name" },
                values: new object[,]
                {
                    { "EUR", true, (byte)2, "Euro" },
                    { "IRR", true, (byte)0, "Iranian Rial" },
                    { "KWD", true, (byte)3, "Kuwaiti Dinar" },
                    { "USD", true, (byte)2, "US Dollar" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currency",
                schema: "reference");
        }
    }
}

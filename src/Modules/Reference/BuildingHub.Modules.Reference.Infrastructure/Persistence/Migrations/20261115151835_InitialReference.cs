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
                    minor_unit_digits = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currency", x => x.code);
                    table.CheckConstraint("ck_currency_minor_unit_digits", "minor_unit_digits BETWEEN 0 AND 3");
                });

            migrationBuilder.InsertData(
                schema: "reference",
                table: "currency",
                columns: new[] { "code", "minor_unit_digits", "name" },
                values: new object[,]
                {
                    { "EUR", (byte)2, "Euro" },
                    { "IRR", (byte)0, "Iranian Rial" },
                    { "KWD", (byte)3, "Kuwaiti Dinar" },
                    { "USD", (byte)2, "US Dollar" }
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

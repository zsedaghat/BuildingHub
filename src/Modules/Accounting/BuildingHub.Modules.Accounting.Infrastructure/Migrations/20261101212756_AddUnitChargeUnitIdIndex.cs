using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitChargeUnitIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UnitCharges_UnitId",
                table: "UnitCharges",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnitCharges_UnitId",
                table: "UnitCharges");
        }
    }
}

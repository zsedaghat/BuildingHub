using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChargeCategoryAndUpdateChargeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Charges_BuildingId_PeriodYear_PeriodMonth",
                table: "Charges");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Charges",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Charges_BuildingId_PeriodYear_PeriodMonth",
                table: "Charges",
                columns: new[] { "BuildingId", "PeriodYear", "PeriodMonth" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Charges_BuildingId_PeriodYear_PeriodMonth",
                table: "Charges");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Charges");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_BuildingId_PeriodYear_PeriodMonth",
                table: "Charges",
                columns: new[] { "BuildingId", "PeriodYear", "PeriodMonth" },
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePaymentTrackingNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payments_TrackingNumber",
                table: "Payments",
                column: "TrackingNumber",
                unique: true,
                filter: "\"TrackingNumber\" IS NOT NULL AND btrim(\"TrackingNumber\") <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_TrackingNumber",
                table: "Payments");
        }
    }
}

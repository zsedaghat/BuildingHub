using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpensesAndChargeExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpenseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargeExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChargeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargeExpenses_Charges_ChargeId",
                        column: x => x.ChargeId,
                        principalTable: "Charges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChargeExpenses_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargeExpenses_ChargeId",
                table: "ChargeExpenses",
                column: "ChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeExpenses_ExpenseId",
                table: "ChargeExpenses",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeExpenses_ExpenseId_ChargeId",
                table: "ChargeExpenses",
                columns: new[] { "ExpenseId", "ChargeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BuildingId",
                table: "Expenses",
                column: "BuildingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeExpenses");

            migrationBuilder.DropTable(
                name: "Expenses");
        }
    }
}


using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingHub.Modules.Reference.Infrastructure.Persistence.Migrations
{
    public partial class AddCurrencyIntegrityRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Remove existing constraints
            migrationBuilder.DropPrimaryKey(
                name: "PK_currency",
                schema: "reference",
                table: "currency");

            migrationBuilder.DropCheckConstraint(
                name: "ck_currency_minor_unit_digits",
                schema: "reference",
                table: "currency");

            // 2. Add IsActive column
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "reference",
                table: "currency",
                type: "boolean",
                nullable: true);

            // 3. Activate existing currencies
            migrationBuilder.Sql("""
                UPDATE reference.currency
                SET is_active = TRUE;
                """);

            // 4. Make IsActive required
            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                schema: "reference",
                table: "currency",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            // 5. Add composite alternate key
            migrationBuilder.AddUniqueConstraint(
                name: "ak_currency_code_minor_unit_digits",
                schema: "reference",
                table: "currency",
                columns: new[]
                {
                    "code",
                    "minor_unit_digits"
                });

            // 6. Recreate primary key with final name
            migrationBuilder.AddPrimaryKey(
                name: "pk_currency",
                schema: "reference",
                table: "currency",
                column: "code");

            // 7. Add currency code constraint
            migrationBuilder.AddCheckConstraint(
                name: "ck_currency_code_uppercase",
                schema: "reference",
                table: "currency",
                sql: "code ~ '^[A-Z]{3}$'");

            // 8. Add minor unit digits constraint
            migrationBuilder.AddCheckConstraint(
                name: "ck_currency_minor_unit_digits",
                schema: "reference",
                table: "currency",
                sql: "minor_unit_digits BETWEEN 0 AND 6");

            // 9. Protect currency code and precision from modification
            // 10. Prevent currency deletion
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION
                    reference.prevent_currency_mutation()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION
                            'Currency records cannot be deleted.'
                            USING ERRCODE = '23514';
                    END IF;

                    IF NEW.code IS DISTINCT FROM OLD.code
                       OR NEW.minor_unit_digits
                            IS DISTINCT FROM OLD.minor_unit_digits
                    THEN
                        RAISE EXCEPTION
                            'Currency code and minor unit digits are immutable.'
                            USING ERRCODE = '23514';
                    END IF;

                    RETURN NEW;
                END;
                $$;

                CREATE TRIGGER trg_currency_prevent_mutation
                BEFORE UPDATE OR DELETE
                ON reference.currency
                FOR EACH ROW
                EXECUTE FUNCTION
                    reference.prevent_currency_mutation();
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Remove trigger and function
            migrationBuilder.Sql("""
                DROP TRIGGER IF EXISTS
                    trg_currency_prevent_mutation
                ON reference.currency;

                DROP FUNCTION IF EXISTS
                    reference.prevent_currency_mutation();
                """);

            // 2. Remove composite alternate key
            migrationBuilder.DropUniqueConstraint(
                name: "ak_currency_code_minor_unit_digits",
                schema: "reference",
                table: "currency");

            // 3. Remove primary key
            migrationBuilder.DropPrimaryKey(
                name: "pk_currency",
                schema: "reference",
                table: "currency");

            // 4. Remove new check constraints
            migrationBuilder.DropCheckConstraint(
                name: "ck_currency_code_uppercase",
                schema: "reference",
                table: "currency");

            migrationBuilder.DropCheckConstraint(
                name: "ck_currency_minor_unit_digits",
                schema: "reference",
                table: "currency");

            // 5. Remove IsActive
            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "reference",
                table: "currency");

            // 6. Restore original primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_currency",
                schema: "reference",
                table: "currency",
                column: "code");

            // 7. Restore original precision constraint
            migrationBuilder.AddCheckConstraint(
                name: "ck_currency_minor_unit_digits",
                schema: "reference",
                table: "currency",
                sql: "minor_unit_digits BETWEEN 0 AND 3");
        }
    }
}

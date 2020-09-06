using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EqlibApi.Migrations
{
    public partial class DefaultCheckoutDate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CheckoutStatus",
                table: "Checkouts",
                nullable: true,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckoutDate",
                table: "Checkouts",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2020, 9, 1, 20, 44, 5, 129, DateTimeKind.Local).AddTicks(8000));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CheckoutStatus",
                table: "Checkouts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 3);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckoutDate",
                table: "Checkouts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2020, 9, 1, 20, 44, 5, 129, DateTimeKind.Local).AddTicks(8000),
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "NOW()");
        }
    }
}

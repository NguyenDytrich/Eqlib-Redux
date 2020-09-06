using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EqlibApi.Migrations
{
    public partial class DefaultCheckoutDateToNow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CheckoutStatus",
                table: "Checkouts",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckoutDate",
                table: "Checkouts",
                nullable: false,
                defaultValue: new DateTime(2020, 9, 1, 20, 44, 5, 129, DateTimeKind.Local).AddTicks(8000),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CheckoutStatus",
                table: "Checkouts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckoutDate",
                table: "Checkouts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 9, 1, 20, 44, 5, 129, DateTimeKind.Local).AddTicks(8000));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace EqlibApi.Migrations
{
    public partial class DefaultCheckoutApprovalStatus1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ApprovalStatus",
                table: "Checkouts",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ApprovalStatus",
                table: "Checkouts",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 1);
        }
    }
}

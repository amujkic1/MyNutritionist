using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class migration12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AspUserId",
                table: "Nutritionist",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AspUserId",
                table: "Nutritionist");
        }
    }
}

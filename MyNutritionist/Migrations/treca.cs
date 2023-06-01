using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class nemojsevisesalit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Nutritionist");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Nutritionist",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class usersmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersID",
                table: "RegisteredUser");

            migrationBuilder.DropColumn(
                name: "UsersID",
                table: "PremiumUser");

            migrationBuilder.DropColumn(
                name: "UsersID",
                table: "Nutritionist");

            migrationBuilder.DropColumn(
                name: "UsersID",
                table: "Admin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsersID",
                table: "RegisteredUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsersID",
                table: "PremiumUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsersID",
                table: "Nutritionist",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsersID",
                table: "Admin",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

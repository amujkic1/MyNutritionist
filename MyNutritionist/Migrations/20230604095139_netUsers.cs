using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class netUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PremiumUser_Nutritionist_NutritionistId",
                table: "PremiumUser");

            migrationBuilder.DropIndex(
                name: "IX_PremiumUser_NutritionistId",
                table: "PremiumUser");

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

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Person",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Person",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_PremiumUser_NutritionistId",
                table: "PremiumUser",
                column: "NutritionistId");

            migrationBuilder.AddForeignKey(
                name: "FK_PremiumUser_Nutritionist_NutritionistId",
                table: "PremiumUser",
                column: "NutritionistId",
                principalTable: "Nutritionist",
                principalColumn: "PID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

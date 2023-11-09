using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_DietPlan_DietPlanDPID",
                table: "Recipe");

            migrationBuilder.DropIndex(
                name: "IX_Recipe_DietPlanDPID",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "DietPlanDPID",
                table: "Recipe");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DietPlanDPID",
                table: "Recipe",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_DietPlanDPID",
                table: "Recipe",
                column: "DietPlanDPID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_DietPlan_DietPlanDPID",
                table: "Recipe",
                column: "DietPlanDPID",
                principalTable: "DietPlan",
                principalColumn: "DPID");
        }
    }
}

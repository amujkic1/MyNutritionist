using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class migration8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DietPlanRecipe",
                table: "DietPlanRecipe");

            migrationBuilder.CreateIndex(
                name: "IX_DietPlanRecipe_DietPlansDPID",
                table: "DietPlanRecipe",
                column: "DietPlansDPID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DietPlanRecipe_DietPlansDPID",
                table: "DietPlanRecipe");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DietPlanRecipe",
                table: "DietPlanRecipe",
                columns: new[] { "DietPlansDPID", "RecipesRID" });
        }
    }
}

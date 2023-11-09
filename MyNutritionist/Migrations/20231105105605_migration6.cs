using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class migration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_DietPlan_DPID",
                table: "DietPlanRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_Recipe_RID",
                table: "DietPlanRecipe");

            migrationBuilder.RenameColumn(
                name: "RID",
                table: "DietPlanRecipe",
                newName: "RecipeRID");

            migrationBuilder.RenameColumn(
                name: "DPID",
                table: "DietPlanRecipe",
                newName: "DietPlanDPID");

            migrationBuilder.RenameIndex(
                name: "IX_DietPlanRecipe_RID",
                table: "DietPlanRecipe",
                newName: "IX_DietPlanRecipe_RecipeRID");

            migrationBuilder.RenameIndex(
                name: "IX_DietPlanRecipe_DPID",
                table: "DietPlanRecipe",
                newName: "IX_DietPlanRecipe_DietPlanDPID");

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanRecipe_DietPlan_DietPlanDPID",
                table: "DietPlanRecipe",
                column: "DietPlanDPID",
                principalTable: "DietPlan",
                principalColumn: "DPID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanRecipe_Recipe_RecipeRID",
                table: "DietPlanRecipe",
                column: "RecipeRID",
                principalTable: "Recipe",
                principalColumn: "RID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_DietPlan_DietPlanDPID",
                table: "DietPlanRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_Recipe_RecipeRID",
                table: "DietPlanRecipe");

            migrationBuilder.RenameColumn(
                name: "RecipeRID",
                table: "DietPlanRecipe",
                newName: "RID");

            migrationBuilder.RenameColumn(
                name: "DietPlanDPID",
                table: "DietPlanRecipe",
                newName: "DPID");

            migrationBuilder.RenameIndex(
                name: "IX_DietPlanRecipe_RecipeRID",
                table: "DietPlanRecipe",
                newName: "IX_DietPlanRecipe_RID");

            migrationBuilder.RenameIndex(
                name: "IX_DietPlanRecipe_DietPlanDPID",
                table: "DietPlanRecipe",
                newName: "IX_DietPlanRecipe_DPID");

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanRecipe_DietPlan_DPID",
                table: "DietPlanRecipe",
                column: "DPID",
                principalTable: "DietPlan",
                principalColumn: "DPID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanRecipe_Recipe_RID",
                table: "DietPlanRecipe",
                column: "RID",
                principalTable: "Recipe",
                principalColumn: "RID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class migration7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_DietPlan_DietPlanDPID",
                table: "DietPlanRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_Recipe_RecipeRID",
                table: "DietPlanRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DietPlanRecipe",
                table: "DietPlanRecipe");

            migrationBuilder.DropIndex(
                name: "IX_DietPlanRecipe_DietPlanDPID",
                table: "DietPlanRecipe");

            migrationBuilder.DropColumn(
                name: "DPRID",
                table: "DietPlanRecipe");

            migrationBuilder.RenameColumn(
                name: "RecipeRID",
                table: "DietPlanRecipe",
                newName: "RecipesRID");

            migrationBuilder.RenameColumn(
                name: "DietPlanDPID",
                table: "DietPlanRecipe",
                newName: "DietPlansDPID");

            migrationBuilder.RenameIndex(
                name: "IX_DietPlanRecipe_RecipeRID",
                table: "DietPlanRecipe",
                newName: "IX_DietPlanRecipe_RecipesRID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DietPlanRecipe",
                table: "DietPlanRecipe",
                columns: new[] { "DietPlansDPID", "RecipesRID" });

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanRecipe_DietPlan_DietPlansDPID",
                table: "DietPlanRecipe",
                column: "DietPlansDPID",
                principalTable: "DietPlan",
                principalColumn: "DPID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DietPlanRecipe_Recipe_RecipesRID",
                table: "DietPlanRecipe",
                column: "RecipesRID",
                principalTable: "Recipe",
                principalColumn: "RID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_DietPlan_DietPlansDPID",
                table: "DietPlanRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_DietPlanRecipe_Recipe_RecipesRID",
                table: "DietPlanRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DietPlanRecipe",
                table: "DietPlanRecipe");

            migrationBuilder.RenameColumn(
                name: "RecipesRID",
                table: "DietPlanRecipe",
                newName: "RecipeRID");

            migrationBuilder.RenameColumn(
                name: "DietPlansDPID",
                table: "DietPlanRecipe",
                newName: "DietPlanDPID");

            migrationBuilder.RenameIndex(
                name: "IX_DietPlanRecipe_RecipesRID",
                table: "DietPlanRecipe",
                newName: "IX_DietPlanRecipe_RecipeRID");

            migrationBuilder.AddColumn<int>(
                name: "DPRID",
                table: "DietPlanRecipe",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DietPlanRecipe",
                table: "DietPlanRecipe",
                column: "DPRID");

            migrationBuilder.CreateIndex(
                name: "IX_DietPlanRecipe_DietPlanDPID",
                table: "DietPlanRecipe",
                column: "DietPlanDPID");

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
    }
}

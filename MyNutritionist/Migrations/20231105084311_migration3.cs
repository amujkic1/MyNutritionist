using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DietPlanRecipe",
                columns: table => new
                {
                    DPRID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RID = table.Column<int>(type: "int", nullable: false),
                    DPID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietPlanRecipe", x => x.DPRID);
                    table.ForeignKey(
                        name: "FK_DietPlanRecipe_DietPlan_DPID",
                        column: x => x.DPID,
                        principalTable: "DietPlan",
                        principalColumn: "DPID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DietPlanRecipe_Recipe_RID",
                        column: x => x.RID,
                        principalTable: "Recipe",
                        principalColumn: "RID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DietPlanRecipe_DPID",
                table: "DietPlanRecipe",
                column: "DPID");

            migrationBuilder.CreateIndex(
                name: "IX_DietPlanRecipe_RID",
                table: "DietPlanRecipe",
                column: "RID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DietPlanRecipe");
        }
    }
}

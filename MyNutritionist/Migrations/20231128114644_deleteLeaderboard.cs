using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNutritionist.Migrations
{
    public partial class deleteLeaderboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PremiumUser_Leaderboard_LeaderboardLID",
                table: "PremiumUser");

            migrationBuilder.DropTable(
                name: "Leaderboard");

            migrationBuilder.DropIndex(
                name: "IX_PremiumUser_LeaderboardLID",
                table: "PremiumUser");

            migrationBuilder.DropColumn(
                name: "LeaderboardLID",
                table: "PremiumUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeaderboardLID",
                table: "PremiumUser",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Leaderboard",
                columns: table => new
                {
                    LID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboard", x => x.LID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PremiumUser_LeaderboardLID",
                table: "PremiumUser",
                column: "LeaderboardLID");

            migrationBuilder.AddForeignKey(
                name: "FK_PremiumUser_Leaderboard_LeaderboardLID",
                table: "PremiumUser",
                column: "LeaderboardLID",
                principalTable: "Leaderboard",
                principalColumn: "LID");
        }
    }
}

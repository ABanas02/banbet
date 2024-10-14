using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace banbet.Migrations
{
    /// <inheritdoc />
    public partial class MajorChangesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OddsID",
                table: "Bets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bets_OddsID",
                table: "Bets",
                column: "OddsID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_Odds_OddsID",
                table: "Bets",
                column: "OddsID",
                principalTable: "Odds",
                principalColumn: "OddsID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bets_Odds_OddsID",
                table: "Bets");

            migrationBuilder.DropIndex(
                name: "IX_Bets_OddsID",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "OddsID",
                table: "Bets");
        }
    }
}

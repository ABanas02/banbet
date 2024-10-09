using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace banbet.Migrations
{
    /// <inheritdoc />
    public partial class BetOddChangew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Teams");

            migrationBuilder.AlterColumn<string>(
                name: "League",
                table: "Teams",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Teams",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "TeamID",
                table: "Odds",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventTeams",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "integer", nullable: false),
                    TeamID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTeams", x => new { x.EventID, x.TeamID });
                    table.ForeignKey(
                        name: "FK_EventTeams_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTeams_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Odds_TeamID",
                table: "Odds",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_EventTeams_TeamID",
                table: "EventTeams",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Odds_Teams_TeamID",
                table: "Odds",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Odds_Teams_TeamID",
                table: "Odds");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players");

            migrationBuilder.DropTable(
                name: "EventTeams");

            migrationBuilder.DropIndex(
                name: "IX_Odds_TeamID",
                table: "Odds");

            migrationBuilder.DropColumn(
                name: "TeamID",
                table: "Odds");

            migrationBuilder.AlterColumn<string>(
                name: "League",
                table: "Teams",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Teams",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Teams",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

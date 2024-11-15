using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace banbet.Migrations
{
    /// <inheritdoc />
    public partial class modelchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Statistics",
                table: "Players");

            migrationBuilder.AddColumn<string>(
                name: "TeamName",
                table: "Odds",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamName",
                table: "Odds");

            migrationBuilder.AddColumn<string>(
                name: "Statistics",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

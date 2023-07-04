using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KillTheFly.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddMovementPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "Kills",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movements",
                table: "Movements",
                column: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kills",
                table: "Kills",
                column: "Guid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Movements",
                table: "Movements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kills",
                table: "Kills");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Kills");
        }
    }
}

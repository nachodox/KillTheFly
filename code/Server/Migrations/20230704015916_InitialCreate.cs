using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KillTheFly.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "text", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Avatar = table.Column<char>(type: "character(1)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAccess = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPlayer = table.Column<bool>(type: "boolean", nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Kills",
                columns: table => new
                {
                    VictimGuid = table.Column<string>(type: "text", nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Kills_Entities_VictimGuid",
                        column: x => x.VictimGuid,
                        principalTable: "Entities",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    MoveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntityGuid = table.Column<string>(type: "text", nullable: true),
                    StartX = table.Column<int>(type: "integer", nullable: false),
                    StartY = table.Column<int>(type: "integer", nullable: false),
                    EndX = table.Column<int>(type: "integer", nullable: false),
                    EndY = table.Column<int>(type: "integer", nullable: false),
                    Guid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Movements_Entities_EntityGuid",
                        column: x => x.EntityGuid,
                        principalTable: "Entities",
                        principalColumn: "Guid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kills_VictimGuid",
                table: "Kills",
                column: "VictimGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_EntityGuid",
                table: "Movements",
                column: "EntityGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kills");

            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
}

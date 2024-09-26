using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KillTheFly.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    GuidClient = table.Column<string>(type: "text", nullable: true),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    StartX = table.Column<int>(type: "integer", nullable: false),
                    StartY = table.Column<int>(type: "integer", nullable: false),
                    Avatar = table.Column<char>(type: "character(1)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsPlayer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Access",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EntityGuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Access", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Access_Entities_EntityGuid",
                        column: x => x.EntityGuid,
                        principalTable: "Entities",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EntityGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    StartX = table.Column<int>(type: "integer", nullable: false),
                    StartY = table.Column<int>(type: "integer", nullable: false),
                    EndX = table.Column<int>(type: "integer", nullable: false),
                    EndY = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Movements_Entities_EntityGuid",
                        column: x => x.EntityGuid,
                        principalTable: "Entities",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kills",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    GameEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kills", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Kills_Entities_GameEntityId",
                        column: x => x.GameEntityId,
                        principalTable: "Entities",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kills_Movements_MovementId",
                        column: x => x.MovementId,
                        principalTable: "Movements",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Access_EntityGuid",
                table: "Access",
                column: "EntityGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Kills_GameEntityId",
                table: "Kills",
                column: "GameEntityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kills_MovementId",
                table: "Kills",
                column: "MovementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movements_EntityGuid",
                table: "Movements",
                column: "EntityGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Access");

            migrationBuilder.DropTable(
                name: "Kills");

            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
}

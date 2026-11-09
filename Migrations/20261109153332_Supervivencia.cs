using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoGalactico.Migrations
{
    /// <inheritdoc />
    public partial class Supervivencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "CardsCharacter",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "SurvivalRatio",
                table: "CardsCharacter",
                type: "int",
                nullable: false,
                defaultValue: 50);

            migrationBuilder.CreateTable(
                name: "BattleRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    CardId = table.Column<int>(type: "int", nullable: true),
                    Outcome = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Roll = table.Column<int>(type: "int", nullable: true),
                    SurvivalRatio = table.Column<int>(type: "int", nullable: true),
                    Reappeared = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleRecords_CardsCharacter_CardId",
                        column: x => x.CardId,
                        principalTable: "CardsCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BattleRecords_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattleRecords_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BattleRecords_CardId",
                table: "BattleRecords",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleRecords_CharacterId",
                table: "BattleRecords",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleRecords_EventId",
                table: "BattleRecords",
                column: "EventId");
            migrationBuilder.Sql(
            "UPDATE CardsCharacter SET SurvivalRatio = 100 - DangerousLevel * 6;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleRecords");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "CardsCharacter");

            migrationBuilder.DropColumn(
                name: "SurvivalRatio",
                table: "CardsCharacter");
        }
    }
}

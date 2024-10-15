using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedding.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Table",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomTable = table.Column<string>(type: "TEXT", nullable: false),
                    NbrePlaces = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    NbreInvitePresent = table.Column<int>(type: "INTEGER", nullable: true),
                    StatutDuJour = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomInvite = table.Column<string>(type: "TEXT", nullable: false),
                    PrenomInvite = table.Column<string>(type: "TEXT", nullable: false),
                    IdInviteur = table.Column<int>(type: "INTEGER", nullable: false),
                    InviteurId = table.Column<int>(type: "INTEGER", nullable: true),
                    IdTable = table.Column<int>(type: "INTEGER", nullable: true),
                    TableId = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeBillets = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invite_Invite_InviteurId",
                        column: x => x.InviteurId,
                        principalTable: "Invite",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invite_Table_TableId",
                        column: x => x.TableId,
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invite_InviteurId",
                table: "Invite",
                column: "InviteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Invite_TableId",
                table: "Invite",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invite");

            migrationBuilder.DropTable(
                name: "Table");
        }
    }
}

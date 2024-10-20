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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomTable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NbrePlaces = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    NbreInvitePresent = table.Column<int>(type: "int", nullable: true),
                    NombreInvites = table.Column<int>(type: "int", nullable: true),
                    StatutDuJour = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomInvite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrenomInvite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdInviteur = table.Column<int>(type: "int", nullable: true),
                    IdTable = table.Column<int>(type: "int", nullable: false),
                    TypeBillets = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QRCodeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invite_Invite_IdInviteur",
                        column: x => x.IdInviteur,
                        principalTable: "Invite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invite_Table_IdTable",
                        column: x => x.IdTable,
                        principalTable: "Table",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invite_IdInviteur",
                table: "Invite",
                column: "IdInviteur");

            migrationBuilder.CreateIndex(
                name: "IX_Invite_IdTable",
                table: "Invite",
                column: "IdTable");
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

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wedding.Migrations
{
    /// <inheritdoc />
    public partial class modelone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Table",
                columns: new[] { "Id", "NbreInvitePresent", "NbrePlaces", "NomTable", "Statut", "StatutDuJour" },
                values: new object[,]
                {
                    { 1, null, 10, "Table 1", 0, 0 },
                    { 2, null, 8, "Table 2", 1, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Table",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Table",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSchema.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueIndexOnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modpacks_Name",
                table: "Modpacks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Licences_Name",
                table: "Licences",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Modpacks_Name",
                table: "Modpacks");

            migrationBuilder.DropIndex(
                name: "IX_Licences_Name",
                table: "Licences");
        }
    }
}

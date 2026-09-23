using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class DatObjectXxHash3Identity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DatObjects_DatName_DatChecksum",
                table: "DatObjects");

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_DatName_DatChecksum",
                table: "DatObjects",
                columns: new[] { "DatName", "DatChecksum" },
                descending: new[] { true, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DatObjects_DatName_DatChecksum",
                table: "DatObjects");

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_DatName_DatChecksum",
                table: "DatObjects",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true,
                descending: new[] { true, false });
        }
    }
}

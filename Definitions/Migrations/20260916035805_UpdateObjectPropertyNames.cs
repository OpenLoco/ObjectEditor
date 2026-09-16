using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateObjectPropertyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "var_04",
                table: "ObjTree",
                newName: "MinHeight");

            migrationBuilder.RenameColumn(
                name: "var_05",
                table: "ObjTree",
                newName: "MaxHeight");

            migrationBuilder.RenameColumn(
                name: "SeasonalVariants",
                table: "ObjTree",
                newName: "VariantFlags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinHeight",
                table: "ObjTree",
                newName: "var_04");

            migrationBuilder.RenameColumn(
                name: "MaxHeight",
                table: "ObjTree",
                newName: "var_05");

            migrationBuilder.RenameColumn(
                name: "VariantFlags",
                table: "ObjTree",
                newName: "SeasonalVariants");
        }
    }
}

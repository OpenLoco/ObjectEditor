using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlignObjectPropertiesWithDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Season",
                table: "ObjTree",
                newName: "CurrentSeason");

            migrationBuilder.RenameColumn(
                name: "CurveSpeed",
                table: "ObjTrack",
                newName: "MaxCurveSpeed");

            migrationBuilder.RenameColumn(
                name: "MaxSpeed",
                table: "ObjRoad",
                newName: "MaxCurveSpeed");

            migrationBuilder.RenameColumn(
                name: "CostFactor",
                table: "ObjLevelCrossing",
                newName: "BuildCostFactor");

            migrationBuilder.RenameColumn(
                name: "FarmIdealSize",
                table: "ObjIndustry",
                newName: "FarmNumFields");

            migrationBuilder.AddColumn<byte>(
                name: "PlatformType",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlatformType",
                table: "ObjTrackStation");

            migrationBuilder.RenameColumn(
                name: "CurrentSeason",
                table: "ObjTree",
                newName: "Season");

            migrationBuilder.RenameColumn(
                name: "MaxCurveSpeed",
                table: "ObjTrack",
                newName: "CurveSpeed");

            migrationBuilder.RenameColumn(
                name: "MaxCurveSpeed",
                table: "ObjRoad",
                newName: "MaxSpeed");

            migrationBuilder.RenameColumn(
                name: "BuildCostFactor",
                table: "ObjLevelCrossing",
                newName: "CostFactor");

            migrationBuilder.RenameColumn(
                name: "FarmNumFields",
                table: "ObjIndustry",
                newName: "FarmIdealSize");
        }
    }
}

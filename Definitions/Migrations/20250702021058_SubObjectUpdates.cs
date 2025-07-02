using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class SubObjectUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flags2",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "ToolId",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "RackRailType",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CargoOffsetBytes",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "CompatibleTrack",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "ManualPower",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "Tunnel",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "CargoTypeId",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "Tunnel",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "CliffEdgeHeader1",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "CliffEdgeHeader2",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "BuildingWall",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BuildingWallEntrance",
                table: "ObjIndustry");

            migrationBuilder.RenameColumn(
                name: "Offset",
                table: "ObjSound",
                newName: "Volume");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "ObjSound",
                newName: "ShouldLoop");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "ObjSound",
                newName: "Offset");

            migrationBuilder.RenameColumn(
                name: "ShouldLoop",
                table: "ObjSound",
                newName: "Length");

            migrationBuilder.AddColumn<byte>(
                name: "Flags2",
                table: "ObjWall",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ToolId",
                table: "ObjWall",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ulong>(
                name: "RackRailType",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "CargoOffsetBytes",
                table: "ObjTrackStation",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompatibleTrack",
                table: "ObjTrackStation",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ManualPower",
                table: "ObjTrackStation",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<ulong>(
                name: "Tunnel",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CargoTypeId",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "Tunnel",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CliffEdgeHeader1",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "CliffEdgeHeader2",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "BuildingWall",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "BuildingWallEntrance",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class SubObjectUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropColumn(
			name: "Flags2",
			table: "ObjWall");

		_ = migrationBuilder.DropColumn(
			name: "ToolId",
			table: "ObjWall");

		_ = migrationBuilder.DropColumn(
			name: "RackRailType",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "CargoOffsetBytes",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "CompatibleTrack",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "ManualPower",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "Tunnel",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "CargoTypeId",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "Tunnel",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "CliffEdgeHeader1",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "CliffEdgeHeader2",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "BuildingWall",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "BuildingWallEntrance",
			table: "ObjIndustry");

		_ = migrationBuilder.RenameColumn(
			name: "Offset",
			table: "ObjSound",
			newName: "Volume");

		_ = migrationBuilder.RenameColumn(
			name: "Length",
			table: "ObjSound",
			newName: "ShouldLoop");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.RenameColumn(
			name: "Volume",
			table: "ObjSound",
			newName: "Offset");

		_ = migrationBuilder.RenameColumn(
			name: "ShouldLoop",
			table: "ObjSound",
			newName: "Length");

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags2",
			table: "ObjWall",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ToolId",
			table: "ObjWall",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "RackRailType",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<string>(
			name: "CargoOffsetBytes",
			table: "ObjTrackStation",
			type: "TEXT",
			nullable: false,
			defaultValue: "");

		_ = migrationBuilder.AddColumn<string>(
			name: "CompatibleTrack",
			table: "ObjTrackStation",
			type: "TEXT",
			nullable: false,
			defaultValue: "");

		_ = migrationBuilder.AddColumn<string>(
			name: "ManualPower",
			table: "ObjTrackStation",
			type: "TEXT",
			nullable: false,
			defaultValue: "");

		_ = migrationBuilder.AddColumn<ulong>(
			name: "Tunnel",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "CargoTypeId",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "Tunnel",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "CliffEdgeHeader1",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "CliffEdgeHeader2",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "BuildingWall",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "BuildingWallEntrance",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);
        }
    }

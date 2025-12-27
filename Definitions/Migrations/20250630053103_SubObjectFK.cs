using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class SubObjectFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjAirport_Objects_Id",
			table: "ObjAirport");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjBridge_Objects_Id",
			table: "ObjBridge");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjBuilding_Objects_Id",
			table: "ObjBuilding");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCargo_Objects_Id",
			table: "ObjCargo");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCliffEdge_Objects_Id",
			table: "ObjCliffEdge");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjClimate_Objects_Id",
			table: "ObjClimate");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCompetitor_Objects_Id",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCurrency_Objects_Id",
			table: "ObjCurrency");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjDock_Objects_Id",
			table: "ObjDock");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjHillShapes_Objects_Id",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjIndustry_Objects_Id",
			table: "ObjIndustry");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjInterface_Objects_Id",
			table: "ObjInterface");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjLand_Objects_Id",
			table: "ObjLand");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjLevelCrossing_Objects_Id",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRegion_Objects_Id",
			table: "ObjRegion");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRoad_Objects_Id",
			table: "ObjRoad");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRoadExtra_Objects_Id",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRoadStation_Objects_Id",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjScaffolding_Objects_Id",
			table: "ObjScaffolding");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjScenarioText_Objects_Id",
			table: "ObjScenarioText");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjSnow_Objects_Id",
			table: "ObjSnow");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjSound_Objects_Id",
			table: "ObjSound");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjSteam_Objects_Id",
			table: "ObjSteam");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjStreetLight_Objects_Id",
			table: "ObjStreetLight");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTownNames_Objects_Id",
			table: "ObjTownNames");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrack_Objects_Id",
			table: "ObjTrack");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrackExtra_Objects_Id",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrackSignal_Objects_Id",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrackStation_Objects_Id",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTree_Objects_Id",
			table: "ObjTree");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTunnel_Objects_Id",
			table: "ObjTunnel");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjVehicle_Objects_Id",
			table: "ObjVehicle");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjWall_Objects_Id",
			table: "ObjWall");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjWater_Objects_Id",
			table: "ObjWater");

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjWater",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjWall",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjTunnel",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjTrackExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjTownNames",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjStreetLight",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjSteam",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjSound",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjSnow",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjScenarioText",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjScaffolding",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjRoadExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjRegion",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjHillShapes",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "SubObjectId",
			table: "Objects",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjCurrency",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjCompetitor",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjClimate",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjCliffEdge",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "ParentId",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjWater_ParentId",
			table: "ObjWater",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjWall_ParentId",
			table: "ObjWall",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjVehicle_ParentId",
			table: "ObjVehicle",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTunnel_ParentId",
			table: "ObjTunnel",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTree_ParentId",
			table: "ObjTree",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrackStation_ParentId",
			table: "ObjTrackStation",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrackSignal_ParentId",
			table: "ObjTrackSignal",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrackExtra_ParentId",
			table: "ObjTrackExtra",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrack_ParentId",
			table: "ObjTrack",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTownNames_ParentId",
			table: "ObjTownNames",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjStreetLight_ParentId",
			table: "ObjStreetLight",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjSteam_ParentId",
			table: "ObjSteam",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjSound_ParentId",
			table: "ObjSound",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjSnow_ParentId",
			table: "ObjSnow",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjScenarioText_ParentId",
			table: "ObjScenarioText",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjScaffolding_ParentId",
			table: "ObjScaffolding",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRoadStation_ParentId",
			table: "ObjRoadStation",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRoadExtra_ParentId",
			table: "ObjRoadExtra",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRoad_ParentId",
			table: "ObjRoad",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRegion_ParentId",
			table: "ObjRegion",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjLevelCrossing_ParentId",
			table: "ObjLevelCrossing",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjLand_ParentId",
			table: "ObjLand",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjInterface_ParentId",
			table: "ObjInterface",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjIndustry_ParentId",
			table: "ObjIndustry",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjHillShapes_ParentId",
			table: "ObjHillShapes",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjDock_ParentId",
			table: "ObjDock",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCurrency_ParentId",
			table: "ObjCurrency",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCompetitor_ParentId",
			table: "ObjCompetitor",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjClimate_ParentId",
			table: "ObjClimate",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCliffEdge_ParentId",
			table: "ObjCliffEdge",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCargo_ParentId",
			table: "ObjCargo",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjBuilding_ParentId",
			table: "ObjBuilding",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjBridge_ParentId",
			table: "ObjBridge",
			column: "ParentId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjAirport_ParentId",
			table: "ObjAirport",
			column: "ParentId");

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjAirport_Objects_ParentId",
			table: "ObjAirport",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjBridge_Objects_ParentId",
			table: "ObjBridge",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjBuilding_Objects_ParentId",
			table: "ObjBuilding",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCargo_Objects_ParentId",
			table: "ObjCargo",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCliffEdge_Objects_ParentId",
			table: "ObjCliffEdge",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjClimate_Objects_ParentId",
			table: "ObjClimate",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCompetitor_Objects_ParentId",
			table: "ObjCompetitor",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCurrency_Objects_ParentId",
			table: "ObjCurrency",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjDock_Objects_ParentId",
			table: "ObjDock",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjHillShapes_Objects_ParentId",
			table: "ObjHillShapes",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjIndustry_Objects_ParentId",
			table: "ObjIndustry",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjInterface_Objects_ParentId",
			table: "ObjInterface",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjLand_Objects_ParentId",
			table: "ObjLand",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjLevelCrossing_Objects_ParentId",
			table: "ObjLevelCrossing",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRegion_Objects_ParentId",
			table: "ObjRegion",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRoad_Objects_ParentId",
			table: "ObjRoad",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRoadExtra_Objects_ParentId",
			table: "ObjRoadExtra",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRoadStation_Objects_ParentId",
			table: "ObjRoadStation",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjScaffolding_Objects_ParentId",
			table: "ObjScaffolding",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjScenarioText_Objects_ParentId",
			table: "ObjScenarioText",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjSnow_Objects_ParentId",
			table: "ObjSnow",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjSound_Objects_ParentId",
			table: "ObjSound",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjSteam_Objects_ParentId",
			table: "ObjSteam",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjStreetLight_Objects_ParentId",
			table: "ObjStreetLight",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTownNames_Objects_ParentId",
			table: "ObjTownNames",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrack_Objects_ParentId",
			table: "ObjTrack",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrackExtra_Objects_ParentId",
			table: "ObjTrackExtra",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrackSignal_Objects_ParentId",
			table: "ObjTrackSignal",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrackStation_Objects_ParentId",
			table: "ObjTrackStation",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTree_Objects_ParentId",
			table: "ObjTree",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTunnel_Objects_ParentId",
			table: "ObjTunnel",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjVehicle_Objects_ParentId",
			table: "ObjVehicle",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjWall_Objects_ParentId",
			table: "ObjWall",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjWater_Objects_ParentId",
			table: "ObjWater",
			column: "ParentId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjAirport_Objects_ParentId",
			table: "ObjAirport");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjBridge_Objects_ParentId",
			table: "ObjBridge");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjBuilding_Objects_ParentId",
			table: "ObjBuilding");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCargo_Objects_ParentId",
			table: "ObjCargo");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCliffEdge_Objects_ParentId",
			table: "ObjCliffEdge");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjClimate_Objects_ParentId",
			table: "ObjClimate");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCompetitor_Objects_ParentId",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjCurrency_Objects_ParentId",
			table: "ObjCurrency");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjDock_Objects_ParentId",
			table: "ObjDock");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjHillShapes_Objects_ParentId",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjIndustry_Objects_ParentId",
			table: "ObjIndustry");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjInterface_Objects_ParentId",
			table: "ObjInterface");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjLand_Objects_ParentId",
			table: "ObjLand");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjLevelCrossing_Objects_ParentId",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRegion_Objects_ParentId",
			table: "ObjRegion");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRoad_Objects_ParentId",
			table: "ObjRoad");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRoadExtra_Objects_ParentId",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjRoadStation_Objects_ParentId",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjScaffolding_Objects_ParentId",
			table: "ObjScaffolding");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjScenarioText_Objects_ParentId",
			table: "ObjScenarioText");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjSnow_Objects_ParentId",
			table: "ObjSnow");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjSound_Objects_ParentId",
			table: "ObjSound");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjSteam_Objects_ParentId",
			table: "ObjSteam");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjStreetLight_Objects_ParentId",
			table: "ObjStreetLight");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTownNames_Objects_ParentId",
			table: "ObjTownNames");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrack_Objects_ParentId",
			table: "ObjTrack");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrackExtra_Objects_ParentId",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrackSignal_Objects_ParentId",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTrackStation_Objects_ParentId",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTree_Objects_ParentId",
			table: "ObjTree");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjTunnel_Objects_ParentId",
			table: "ObjTunnel");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjVehicle_Objects_ParentId",
			table: "ObjVehicle");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjWall_Objects_ParentId",
			table: "ObjWall");

		_ = migrationBuilder.DropForeignKey(
			name: "FK_ObjWater_Objects_ParentId",
			table: "ObjWater");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjWater_ParentId",
			table: "ObjWater");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjWall_ParentId",
			table: "ObjWall");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjVehicle_ParentId",
			table: "ObjVehicle");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTunnel_ParentId",
			table: "ObjTunnel");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTree_ParentId",
			table: "ObjTree");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrackStation_ParentId",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrackSignal_ParentId",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrackExtra_ParentId",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrack_ParentId",
			table: "ObjTrack");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTownNames_ParentId",
			table: "ObjTownNames");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjStreetLight_ParentId",
			table: "ObjStreetLight");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjSteam_ParentId",
			table: "ObjSteam");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjSound_ParentId",
			table: "ObjSound");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjSnow_ParentId",
			table: "ObjSnow");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjScenarioText_ParentId",
			table: "ObjScenarioText");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjScaffolding_ParentId",
			table: "ObjScaffolding");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRoadStation_ParentId",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRoadExtra_ParentId",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRoad_ParentId",
			table: "ObjRoad");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRegion_ParentId",
			table: "ObjRegion");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjLevelCrossing_ParentId",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjLand_ParentId",
			table: "ObjLand");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjInterface_ParentId",
			table: "ObjInterface");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjIndustry_ParentId",
			table: "ObjIndustry");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjHillShapes_ParentId",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjDock_ParentId",
			table: "ObjDock");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCurrency_ParentId",
			table: "ObjCurrency");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCompetitor_ParentId",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjClimate_ParentId",
			table: "ObjClimate");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCliffEdge_ParentId",
			table: "ObjCliffEdge");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCargo_ParentId",
			table: "ObjCargo");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjBuilding_ParentId",
			table: "ObjBuilding");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjBridge_ParentId",
			table: "ObjBridge");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjAirport_ParentId",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjWater");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjWall");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjTunnel");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjTownNames");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjStreetLight");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjSteam");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjSound");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjSnow");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjScenarioText");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjScaffolding");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjRegion");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropColumn(
			name: "SubObjectId",
			table: "Objects");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjCurrency");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjClimate");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjCliffEdge");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "ParentId",
			table: "ObjAirport");

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjAirport_Objects_Id",
			table: "ObjAirport",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjBridge_Objects_Id",
			table: "ObjBridge",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjBuilding_Objects_Id",
			table: "ObjBuilding",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCargo_Objects_Id",
			table: "ObjCargo",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCliffEdge_Objects_Id",
			table: "ObjCliffEdge",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjClimate_Objects_Id",
			table: "ObjClimate",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCompetitor_Objects_Id",
			table: "ObjCompetitor",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjCurrency_Objects_Id",
			table: "ObjCurrency",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjDock_Objects_Id",
			table: "ObjDock",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjHillShapes_Objects_Id",
			table: "ObjHillShapes",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjIndustry_Objects_Id",
			table: "ObjIndustry",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjInterface_Objects_Id",
			table: "ObjInterface",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjLand_Objects_Id",
			table: "ObjLand",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjLevelCrossing_Objects_Id",
			table: "ObjLevelCrossing",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRegion_Objects_Id",
			table: "ObjRegion",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRoad_Objects_Id",
			table: "ObjRoad",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRoadExtra_Objects_Id",
			table: "ObjRoadExtra",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjRoadStation_Objects_Id",
			table: "ObjRoadStation",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjScaffolding_Objects_Id",
			table: "ObjScaffolding",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjScenarioText_Objects_Id",
			table: "ObjScenarioText",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjSnow_Objects_Id",
			table: "ObjSnow",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjSound_Objects_Id",
			table: "ObjSound",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjSteam_Objects_Id",
			table: "ObjSteam",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjStreetLight_Objects_Id",
			table: "ObjStreetLight",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTownNames_Objects_Id",
			table: "ObjTownNames",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrack_Objects_Id",
			table: "ObjTrack",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrackExtra_Objects_Id",
			table: "ObjTrackExtra",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrackSignal_Objects_Id",
			table: "ObjTrackSignal",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTrackStation_Objects_Id",
			table: "ObjTrackStation",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTree_Objects_Id",
			table: "ObjTree",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjTunnel_Objects_Id",
			table: "ObjTunnel",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjVehicle_Objects_Id",
			table: "ObjVehicle",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjWall_Objects_Id",
			table: "ObjWall",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);

		_ = migrationBuilder.AddForeignKey(
			name: "FK_ObjWater_Objects_Id",
			table: "ObjWater",
			column: "Id",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);
        }
    }

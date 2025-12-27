using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class SubObjectFKIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjWater_Id",
			table: "ObjWater",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjWall_Id",
			table: "ObjWall",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjVehicle_Id",
			table: "ObjVehicle",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTunnel_Id",
			table: "ObjTunnel",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTree_Id",
			table: "ObjTree",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrackStation_Id",
			table: "ObjTrackStation",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrackSignal_Id",
			table: "ObjTrackSignal",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrackExtra_Id",
			table: "ObjTrackExtra",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTrack_Id",
			table: "ObjTrack",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjTownNames_Id",
			table: "ObjTownNames",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjStreetLight_Id",
			table: "ObjStreetLight",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjSteam_Id",
			table: "ObjSteam",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjSound_Id",
			table: "ObjSound",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjSnow_Id",
			table: "ObjSnow",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjScenarioText_Id",
			table: "ObjScenarioText",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjScaffolding_Id",
			table: "ObjScaffolding",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRoadStation_Id",
			table: "ObjRoadStation",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRoadExtra_Id",
			table: "ObjRoadExtra",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRoad_Id",
			table: "ObjRoad",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjRegion_Id",
			table: "ObjRegion",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjLevelCrossing_Id",
			table: "ObjLevelCrossing",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjLand_Id",
			table: "ObjLand",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjInterface_Id",
			table: "ObjInterface",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjIndustry_Id",
			table: "ObjIndustry",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjHillShapes_Id",
			table: "ObjHillShapes",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjDock_Id",
			table: "ObjDock",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCurrency_Id",
			table: "ObjCurrency",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCompetitor_Id",
			table: "ObjCompetitor",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjClimate_Id",
			table: "ObjClimate",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCliffEdge_Id",
			table: "ObjCliffEdge",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjCargo_Id",
			table: "ObjCargo",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjBuilding_Id",
			table: "ObjBuilding",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjBridge_Id",
			table: "ObjBridge",
			column: "Id",
			unique: true);

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjAirport_Id",
			table: "ObjAirport",
			column: "Id",
			unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropIndex(
			name: "IX_ObjWater_Id",
			table: "ObjWater");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjWall_Id",
			table: "ObjWall");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjVehicle_Id",
			table: "ObjVehicle");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTunnel_Id",
			table: "ObjTunnel");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTree_Id",
			table: "ObjTree");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrackStation_Id",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrackSignal_Id",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrackExtra_Id",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTrack_Id",
			table: "ObjTrack");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjTownNames_Id",
			table: "ObjTownNames");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjStreetLight_Id",
			table: "ObjStreetLight");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjSteam_Id",
			table: "ObjSteam");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjSound_Id",
			table: "ObjSound");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjSnow_Id",
			table: "ObjSnow");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjScenarioText_Id",
			table: "ObjScenarioText");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjScaffolding_Id",
			table: "ObjScaffolding");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRoadStation_Id",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRoadExtra_Id",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRoad_Id",
			table: "ObjRoad");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjRegion_Id",
			table: "ObjRegion");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjLevelCrossing_Id",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjLand_Id",
			table: "ObjLand");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjInterface_Id",
			table: "ObjInterface");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjIndustry_Id",
			table: "ObjIndustry");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjHillShapes_Id",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjDock_Id",
			table: "ObjDock");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCurrency_Id",
			table: "ObjCurrency");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCompetitor_Id",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjClimate_Id",
			table: "ObjClimate");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCliffEdge_Id",
			table: "ObjCliffEdge");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjCargo_Id",
			table: "ObjCargo");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjBuilding_Id",
			table: "ObjBuilding");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjBridge_Id",
			table: "ObjBridge");

		_ = migrationBuilder.DropIndex(
			name: "IX_ObjAirport_Id",
			table: "ObjAirport");
        }
    }

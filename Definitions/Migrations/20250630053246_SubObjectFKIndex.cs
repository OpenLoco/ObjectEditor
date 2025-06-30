using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class SubObjectFKIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ObjWater_Id",
                table: "ObjWater",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjWall_Id",
                table: "ObjWall",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjVehicle_Id",
                table: "ObjVehicle",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjTunnel_Id",
                table: "ObjTunnel",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjTree_Id",
                table: "ObjTree",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackStation_Id",
                table: "ObjTrackStation",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackSignal_Id",
                table: "ObjTrackSignal",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrackExtra_Id",
                table: "ObjTrackExtra",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjTrack_Id",
                table: "ObjTrack",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjTownNames_Id",
                table: "ObjTownNames",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjStreetLight_Id",
                table: "ObjStreetLight",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjSteam_Id",
                table: "ObjSteam",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjSound_Id",
                table: "ObjSound",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjSnow_Id",
                table: "ObjSnow",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjScenarioText_Id",
                table: "ObjScenarioText",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjScaffolding_Id",
                table: "ObjScaffolding",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoadStation_Id",
                table: "ObjRoadStation",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoadExtra_Id",
                table: "ObjRoadExtra",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjRoad_Id",
                table: "ObjRoad",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjRegion_Id",
                table: "ObjRegion",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjLevelCrossing_Id",
                table: "ObjLevelCrossing",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjLand_Id",
                table: "ObjLand",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjInterface_Id",
                table: "ObjInterface",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjIndustry_Id",
                table: "ObjIndustry",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjHillShapes_Id",
                table: "ObjHillShapes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjDock_Id",
                table: "ObjDock",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjCurrency_Id",
                table: "ObjCurrency",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjCompetitor_Id",
                table: "ObjCompetitor",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjClimate_Id",
                table: "ObjClimate",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjCliffEdge_Id",
                table: "ObjCliffEdge",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjCargo_Id",
                table: "ObjCargo",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjBuilding_Id",
                table: "ObjBuilding",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjBridge_Id",
                table: "ObjBridge",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjAirport_Id",
                table: "ObjAirport",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ObjWater_Id",
                table: "ObjWater");

            migrationBuilder.DropIndex(
                name: "IX_ObjWall_Id",
                table: "ObjWall");

            migrationBuilder.DropIndex(
                name: "IX_ObjVehicle_Id",
                table: "ObjVehicle");

            migrationBuilder.DropIndex(
                name: "IX_ObjTunnel_Id",
                table: "ObjTunnel");

            migrationBuilder.DropIndex(
                name: "IX_ObjTree_Id",
                table: "ObjTree");

            migrationBuilder.DropIndex(
                name: "IX_ObjTrackStation_Id",
                table: "ObjTrackStation");

            migrationBuilder.DropIndex(
                name: "IX_ObjTrackSignal_Id",
                table: "ObjTrackSignal");

            migrationBuilder.DropIndex(
                name: "IX_ObjTrackExtra_Id",
                table: "ObjTrackExtra");

            migrationBuilder.DropIndex(
                name: "IX_ObjTrack_Id",
                table: "ObjTrack");

            migrationBuilder.DropIndex(
                name: "IX_ObjTownNames_Id",
                table: "ObjTownNames");

            migrationBuilder.DropIndex(
                name: "IX_ObjStreetLight_Id",
                table: "ObjStreetLight");

            migrationBuilder.DropIndex(
                name: "IX_ObjSteam_Id",
                table: "ObjSteam");

            migrationBuilder.DropIndex(
                name: "IX_ObjSound_Id",
                table: "ObjSound");

            migrationBuilder.DropIndex(
                name: "IX_ObjSnow_Id",
                table: "ObjSnow");

            migrationBuilder.DropIndex(
                name: "IX_ObjScenarioText_Id",
                table: "ObjScenarioText");

            migrationBuilder.DropIndex(
                name: "IX_ObjScaffolding_Id",
                table: "ObjScaffolding");

            migrationBuilder.DropIndex(
                name: "IX_ObjRoadStation_Id",
                table: "ObjRoadStation");

            migrationBuilder.DropIndex(
                name: "IX_ObjRoadExtra_Id",
                table: "ObjRoadExtra");

            migrationBuilder.DropIndex(
                name: "IX_ObjRoad_Id",
                table: "ObjRoad");

            migrationBuilder.DropIndex(
                name: "IX_ObjRegion_Id",
                table: "ObjRegion");

            migrationBuilder.DropIndex(
                name: "IX_ObjLevelCrossing_Id",
                table: "ObjLevelCrossing");

            migrationBuilder.DropIndex(
                name: "IX_ObjLand_Id",
                table: "ObjLand");

            migrationBuilder.DropIndex(
                name: "IX_ObjInterface_Id",
                table: "ObjInterface");

            migrationBuilder.DropIndex(
                name: "IX_ObjIndustry_Id",
                table: "ObjIndustry");

            migrationBuilder.DropIndex(
                name: "IX_ObjHillShapes_Id",
                table: "ObjHillShapes");

            migrationBuilder.DropIndex(
                name: "IX_ObjDock_Id",
                table: "ObjDock");

            migrationBuilder.DropIndex(
                name: "IX_ObjCurrency_Id",
                table: "ObjCurrency");

            migrationBuilder.DropIndex(
                name: "IX_ObjCompetitor_Id",
                table: "ObjCompetitor");

            migrationBuilder.DropIndex(
                name: "IX_ObjClimate_Id",
                table: "ObjClimate");

            migrationBuilder.DropIndex(
                name: "IX_ObjCliffEdge_Id",
                table: "ObjCliffEdge");

            migrationBuilder.DropIndex(
                name: "IX_ObjCargo_Id",
                table: "ObjCargo");

            migrationBuilder.DropIndex(
                name: "IX_ObjBuilding_Id",
                table: "ObjBuilding");

            migrationBuilder.DropIndex(
                name: "IX_ObjBridge_Id",
                table: "ObjBridge");

            migrationBuilder.DropIndex(
                name: "IX_ObjAirport_Id",
                table: "ObjAirport");
        }
    }
}

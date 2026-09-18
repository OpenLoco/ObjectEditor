using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class ObjectSubTableProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompatibleRoadObjectCount",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "BoatPositionX",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "BoatPositionY",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "NumBuildingPartAnimations",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "NumBuildingVariationParts",
                table: "ObjDock");

            migrationBuilder.RenameColumn(
                name: "NumRequiredTrackExtras",
                table: "ObjVehicle",
                newName: "NumSimultaneousCargoTypes");

            migrationBuilder.RenameColumn(
                name: "NumCompatibleVehicles",
                table: "ObjVehicle",
                newName: "CompanyColourSchemeIndex");

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

            migrationBuilder.AddColumn<string>(
                name: "BodySprites",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BogieSprites",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CarComponents",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoTypeSpriteOffsets",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompatibleCargoCategories",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompatibleVehicles",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CrossingSounds",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrivingSound",
                table: "ObjVehicle",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrictionSound",
                table: "ObjVehicle",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GearboxMotorSound",
                table: "ObjVehicle",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaxCargo",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ParticleEmitters",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RackRail",
                table: "ObjVehicle",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredTrackExtras",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoadOrTrackType",
                table: "ObjVehicle",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimpleMotorSound",
                table: "ObjVehicle",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartSounds",
                table: "ObjVehicle",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoOffsets",
                table: "ObjTrackStation",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompatibleTrackObjects",
                table: "ObjTrackStation",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiagonalCargoOffsetBytes",
                table: "ObjTrackStation",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompatibleTrackObjects",
                table: "ObjTrackSignal",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bridges",
                table: "ObjTrack",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Signals",
                table: "ObjTrack",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Stations",
                table: "ObjTrack",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrackMods",
                table: "ObjTrack",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TracksAndRoads",
                table: "ObjTrack",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tunnel",
                table: "ObjTrack",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "var_06",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "MorphemeCategories",
                table: "ObjTownNames",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DesignedYears",
                table: "ObjStreetLight",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "FrameInfoType0",
                table: "ObjSteam",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrameInfoType1",
                table: "ObjSteam",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<uint>(
                name: "ImageOffset",
                table: "ObjSteam",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<string>(
                name: "SoundEffects",
                table: "ObjSteam",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoundObjectData",
                table: "ObjSound",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoofHeights",
                table: "ObjScaffolding",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SegmentHeights",
                table: "ObjScaffolding",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "CargoOffsets",
                table: "ObjRoadStation",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoType",
                table: "ObjRoadStation",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompatibleRoadObjects",
                table: "ObjRoadStation",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bridges",
                table: "ObjRoad",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoadMods",
                table: "ObjRoad",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Stations",
                table: "ObjRoad",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TracksAndRoads",
                table: "ObjRoad",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tunnel",
                table: "ObjRoad",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoInfluenceObjects",
                table: "ObjRegion",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoInfluenceTownFilter",
                table: "ObjRegion",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "DependentObjects",
                table: "ObjRegion",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<ushort>(
                name: "VehicleDrivingSide",
                table: "ObjRegion",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<string>(
                name: "CliffEdgeHeader",
                table: "ObjLand",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReplacementLandHeader",
                table: "ObjLand",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnimationSequences",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BuildingComponents",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BuildingWall",
                table: "ObjIndustry",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingWallEntrance",
                table: "ObjIndustry",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Buildings",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "InitialProductionRate",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "NumFarmTileImages",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "ProducedCargo",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RandomAnimations",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequiredCargo",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WallTypes",
                table: "ObjIndustry",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BoatPosition",
                table: "ObjDock",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BuildingComponents",
                table: "ObjDock",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<ushort>(
                name: "UnitWeight",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<string>(
                name: "BuildingComponents",
                table: "ObjBuilding",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConsumedCargoQuantity",
                table: "ObjBuilding",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ConsumedCargoType",
                table: "ObjBuilding",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ElevatorHeightSequences",
                table: "ObjBuilding",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProducedCargoQuantity",
                table: "ObjBuilding",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ProducedCargoType",
                table: "ObjBuilding",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProducedQuantity",
                table: "ObjBuilding",
                type: "json",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<byte>(
                name: "TownAmenityCategory",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "CompatibleRoadObjects",
                table: "ObjBridge",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompatibleTrackObjects",
                table: "ObjBridge",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "var_03",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "BuildingComponents",
                table: "ObjAirport",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BuildingPositions",
                table: "ObjAirport",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MovementEdges",
                table: "ObjAirport",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MovementNodes",
                table: "ObjAirport",
                type: "json",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<uint>(
                name: "RequiredClearEdges",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flags2",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "ToolId",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "BodySprites",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "BogieSprites",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CarComponents",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CargoTypeSpriteOffsets",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CompatibleCargoCategories",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CompatibleVehicles",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CrossingSounds",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "DrivingSound",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "FrictionSound",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "GearboxMotorSound",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "MaxCargo",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "ParticleEmitters",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "RackRail",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "RequiredTrackExtras",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "RoadOrTrackType",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "SimpleMotorSound",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "StartSounds",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CargoOffsets",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "CompatibleTrackObjects",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "DiagonalCargoOffsetBytes",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "CompatibleTrackObjects",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "Bridges",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "Signals",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "Stations",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "TrackMods",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "TracksAndRoads",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "Tunnel",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "var_06",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "MorphemeCategories",
                table: "ObjTownNames");

            migrationBuilder.DropColumn(
                name: "DesignedYears",
                table: "ObjStreetLight");

            migrationBuilder.DropColumn(
                name: "FrameInfoType0",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "FrameInfoType1",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "ImageOffset",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "SoundEffects",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "SoundObjectData",
                table: "ObjSound");

            migrationBuilder.DropColumn(
                name: "RoofHeights",
                table: "ObjScaffolding");

            migrationBuilder.DropColumn(
                name: "SegmentHeights",
                table: "ObjScaffolding");

            migrationBuilder.DropColumn(
                name: "CargoOffsets",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "CargoType",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "CompatibleRoadObjects",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "Bridges",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "RoadMods",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "Stations",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "TracksAndRoads",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "Tunnel",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "CargoInfluenceObjects",
                table: "ObjRegion");

            migrationBuilder.DropColumn(
                name: "CargoInfluenceTownFilter",
                table: "ObjRegion");

            migrationBuilder.DropColumn(
                name: "DependentObjects",
                table: "ObjRegion");

            migrationBuilder.DropColumn(
                name: "VehicleDrivingSide",
                table: "ObjRegion");

            migrationBuilder.DropColumn(
                name: "CliffEdgeHeader",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "ReplacementLandHeader",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "AnimationSequences",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BuildingComponents",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BuildingWall",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BuildingWallEntrance",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "Buildings",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "InitialProductionRate",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "NumFarmTileImages",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "ProducedCargo",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "RandomAnimations",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "RequiredCargo",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "WallTypes",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BoatPosition",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "BuildingComponents",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "UnitWeight",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "BuildingComponents",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ConsumedCargoQuantity",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ConsumedCargoType",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ElevatorHeightSequences",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ProducedCargoQuantity",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ProducedCargoType",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ProducedQuantity",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "TownAmenityCategory",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "CompatibleRoadObjects",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "CompatibleTrackObjects",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "var_03",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "BuildingComponents",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "BuildingPositions",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "MovementEdges",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "MovementNodes",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "RequiredClearEdges",
                table: "ObjAirport");

            migrationBuilder.RenameColumn(
                name: "NumSimultaneousCargoTypes",
                table: "ObjVehicle",
                newName: "NumRequiredTrackExtras");

            migrationBuilder.RenameColumn(
                name: "CompanyColourSchemeIndex",
                table: "ObjVehicle",
                newName: "NumCompatibleVehicles");

            migrationBuilder.AddColumn<byte>(
                name: "CompatibleRoadObjectCount",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "BoatPositionX",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "BoatPositionY",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumBuildingPartAnimations",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumBuildingVariationParts",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}

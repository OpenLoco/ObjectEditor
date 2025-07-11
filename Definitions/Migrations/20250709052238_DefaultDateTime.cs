using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class DefaultDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "SC5Files",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "date('now')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "SC5FilePacks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "date('now')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "Objects",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "date('now')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "ObjectPacks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "date('now')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjWater_Objects_ParentId",
                table: "ObjWater");

            migrationBuilder.DropTable(
                name: "ObjAirport");

            migrationBuilder.DropTable(
                name: "ObjBridge");

            migrationBuilder.DropTable(
                name: "ObjBuilding");

            migrationBuilder.DropTable(
                name: "ObjCargo");

            migrationBuilder.DropTable(
                name: "ObjCliffEdge");

            migrationBuilder.DropTable(
                name: "ObjClimate");

            migrationBuilder.DropTable(
                name: "ObjCompetitor");

            migrationBuilder.DropTable(
                name: "ObjCurrency");

            migrationBuilder.DropTable(
                name: "ObjDock");

            migrationBuilder.DropTable(
                name: "ObjHillShapes");

            migrationBuilder.DropTable(
                name: "ObjIndustry");

            migrationBuilder.DropTable(
                name: "ObjInterface");

            migrationBuilder.DropTable(
                name: "ObjLand");

            migrationBuilder.DropTable(
                name: "ObjLevelCrossing");

            migrationBuilder.DropTable(
                name: "ObjRegion");

            migrationBuilder.DropTable(
                name: "ObjRoad");

            migrationBuilder.DropTable(
                name: "ObjRoadExtra");

            migrationBuilder.DropTable(
                name: "ObjRoadStation");

            migrationBuilder.DropTable(
                name: "ObjScaffolding");

            migrationBuilder.DropTable(
                name: "ObjScenarioText");

            migrationBuilder.DropTable(
                name: "ObjSnow");

            migrationBuilder.DropTable(
                name: "ObjSound");

            migrationBuilder.DropTable(
                name: "ObjSteam");

            migrationBuilder.DropTable(
                name: "ObjStreetLight");

            migrationBuilder.DropTable(
                name: "ObjTownNames");

            migrationBuilder.DropTable(
                name: "ObjTrack");

            migrationBuilder.DropTable(
                name: "ObjTrackExtra");

            migrationBuilder.DropTable(
                name: "ObjTrackSignal");

            migrationBuilder.DropTable(
                name: "ObjTrackStation");

            migrationBuilder.DropTable(
                name: "ObjTree");

            migrationBuilder.DropTable(
                name: "ObjTunnel");

            migrationBuilder.DropTable(
                name: "ObjVehicle");

            migrationBuilder.DropTable(
                name: "ObjWall");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObjWater",
                table: "ObjWater");

            migrationBuilder.DropIndex(
                name: "IX_ObjWater_ParentId",
                table: "ObjWater");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ObjWater");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "SC5Files",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "date('now')");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "SC5FilePacks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "date('now')");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "Objects",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "date('now')");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UploadedDate",
                table: "ObjectPacks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime(datetime('now', 'localtime'), 'utc')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "date('now')");

            migrationBuilder.AlterColumn<byte>(
                name: "CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<short>(
                name: "CostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<byte>(
                name: "Aggressiveness",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "AllowedPlaneTypes",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "AnimationSpeed",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "AvailableNamePrefixes",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "AvailablePlaystyles",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "AverageNumberOnMap",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "BaseCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "BoatPositionX",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "BoatPositionY",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "BuildingSizeFlags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "CargoCategory",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "CargoTransferTime",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ClearCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "ClearHeight",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Clearance",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ClosedFrames",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ClosingFrames",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Colour_11",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Colours",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CompatibleRoadObjectCount",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Competitiveness",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CurveSpeed",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DeckDepth",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DemolishRatingReduction",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "DisabledTrackFlags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "DbSubObject",
                type: "TEXT",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "DisplayOffset",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "DistributionPattern",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "DrivingSoundType",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Emotions",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ErrorColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Factor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "FarmGrowthStageWithNoProduction",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "FarmIdealSize",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "FarmImagesPerGrowthStage",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "FarmNumStagesOfGrowth",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "FarmTileNumImageAngles",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "FirstSeason",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Flags1",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "GeneratorFunction",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Height",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "HeightCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "HillHeightMapCount",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Intelligence",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "LargeTiles",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MapColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MapTooltipCargoColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MapTooltipObjectColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MaxHeight",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MaxNonPremiumDays",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MaxNumBuildings",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "MaxPremiumRate",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "MaxSpeed",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "MaxX",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "MaxY",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MinNumBuildings",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "MinX",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "MinY",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Mode",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MonthlyClosureChance",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "MountainHeightMapCount",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumBuildingPartAnimations",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumBuildingVariationParts",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumCarComponents",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumCompatibleVehicles",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumFrames",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumGrowthStages",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumImageAngles",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "NumImagesPerGrowthStage",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumPlatformVariations",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumRequiredTrackExtras",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumRotations",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumStationaryTicks",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NumVariations",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PaintStyle",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "PaymentFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PaymentIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "PenaltyRate",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PillarSpacing",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PlayerInfoToolbarColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "Power",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PremiumDays",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "RackSpeed",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Rating",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Reliability",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "RoadPieces",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "RunCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RunCostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ScaffoldingColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ScaffoldingSegmentType",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Season",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SeasonLength1",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SeasonLength2",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SeasonLength3",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SeasonLength4",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SeasonState",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Separator",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "ShadowImageOffset",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ShipWakeOffset",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ShouldLoop",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpanLength",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Speed",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpriteHeightNegative",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpriteHeightPositive",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpriteWidth",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "StationCargoDensity",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "StationTrackPieces",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SummerSnowLine",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TargetTownSize",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectBridge_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectBridge_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectBridge_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectBuilding_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectBuilding_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectBuilding_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectBuilding_ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectBuilding_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectCargo_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectDock_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectDock_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectDock_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectDock_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectDock_ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectDock_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectHillShapes_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectIndustry_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "TblObjectIndustry_Colours",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectIndustry_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectIndustry_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "TblObjectIndustry_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectIndustry_ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectIndustry_ScaffoldingColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectIndustry_ScaffoldingSegmentType",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectIndustry_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectLand_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectLand_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectLevelCrossing_CostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectLevelCrossing_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectLevelCrossing_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectLevelCrossing_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectRoadExtra_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectRoadExtra_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectRoadExtra_PaintStyle",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectRoadExtra_RoadPieces",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectRoadExtra_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectRoadStation_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectRoadStation_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectRoadStation_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectRoadStation_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectRoadStation_ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectRoadStation_PaintStyle",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectRoadStation_RoadPieces",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectRoadStation_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectRoad_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectRoad_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectRoad_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectRoad_MaxSpeed",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectRoad_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectSteam_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrackExtra_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackExtra_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackExtra_PaintStyle",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrackExtra_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrackExtra_TrackPieces",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackSignal_AnimationSpeed",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrackSignal_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackSignal_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrackSignal_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrackSignal_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrackSignal_ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrackSignal_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrackStation_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackStation_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrackStation_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackStation_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackStation_Height",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrackStation_ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrackStation_PaintStyle",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrackStation_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrackStation_TrackPieces",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrack_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrack_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTrack_DisplayOffset",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTrack_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrack_SellCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTrack_TunnelCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTree_BuildCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "TblObjectTree_Colours",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTree_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectTree_DemolishRatingReduction",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectTree_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTree_Height",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectTree_NumGrowthStages",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectVehicle_CostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectVehicle_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectVehicle_DesignedYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectVehicle_Flags",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TblObjectVehicle_ObsoleteYear",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectWall_Height",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TblObjectWater_CostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TblObjectWater_CostIndex",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TimeToolbarColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TooltipColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarPrimaryColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarQuaternaryColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarSecondaryColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarTertiaryColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TotalOfTypeInScenario",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "TrackPieces",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TrackTypeId",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "TunnelCostFactor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "UnitSize",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "VariationLikelihood",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Volume",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "Weight",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WindowColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WindowConstructionColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WindowMapColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WindowOptionsColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WindowPlayerColor",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WindowTerraFormColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WindowTitlebarColour",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WinterSnowLine",
                table: "DbSubObject",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DbSubObject",
                table: "DbSubObject",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_SubObjectId",
                table: "Objects",
                column: "SubObjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Objects_DbSubObject_SubObjectId",
                table: "Objects",
                column: "SubObjectId",
                principalTable: "DbSubObject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

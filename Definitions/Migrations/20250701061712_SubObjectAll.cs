using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoco.Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class SubObjectAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "CostFactor",
                table: "ObjWater",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjWater",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags1",
                table: "ObjWall",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags2",
                table: "ObjWall",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Height",
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

            migrationBuilder.AddColumn<short>(
                name: "CostFactor",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "DrivingSoundType",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Mode",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumCarComponents",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumCompatibleVehicles",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumRequiredTrackExtras",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Power",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ulong>(
                name: "RackRailType",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<short>(
                name: "RackSpeed",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "Reliability",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "RunCostFactor",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "RunCostIndex",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ShipWakeOffset",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "Speed",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "TrackTypeId",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Weight",
                table: "ObjVehicle",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "ClearCostFactor",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "Clearance",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "Colours",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "DemolishRatingReduction",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Height",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumGrowthStages",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumRotations",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "Rating",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "Season",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SeasonState",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ShadowImageOffset",
                table: "ObjTree",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "CargoOffsetBytes",
                table: "ObjTrackStation",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "CompatibleTrack",
                table: "ObjTrackStation",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Height",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "ManualPower",
                table: "ObjTrackStation",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "PaintStyle",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "TrackPieces",
                table: "ObjTrackStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "AnimationSpeed",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumFrames",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjTrackSignal",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjTrackExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjTrackExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PaintStyle",
                table: "ObjTrackExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjTrackExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "TrackPieces",
                table: "ObjTrackExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "CurveSpeed",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "DisplayOffset",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "StationTrackPieces",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "TrackPieces",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ulong>(
                name: "Tunnel",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<short>(
                name: "TunnelCostFactor",
                table: "ObjTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjSteam",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumStationaryTicks",
                table: "ObjSteam",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SpriteHeightNegative",
                table: "ObjSteam",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SpriteHeightPositive",
                table: "ObjSteam",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SpriteWidth",
                table: "ObjSteam",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "Length",
                table: "ObjSound",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<int>(
                name: "Offset",
                table: "ObjSound",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ulong>(
                name: "CargoTypeId",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<byte>(
                name: "CompatibleRoadObjectCount",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Height",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "PaintStyle",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "RoadPieces",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjRoadStation",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjRoadExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjRoadExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PaintStyle",
                table: "ObjRoadExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "RoadPieces",
                table: "ObjRoadExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjRoadExtra",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DisplayOffset",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "MaxSpeed",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "PaintStyle",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "RoadPieces",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "TargetTownSize",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ulong>(
                name: "Tunnel",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<short>(
                name: "TunnelCostFactor",
                table: "ObjRoad",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "AnimationSpeed",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ClosedFrames",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ClosingFrames",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "CostFactor",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjLevelCrossing",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

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

            migrationBuilder.AddColumn<short>(
                name: "CostFactor",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DistributionPattern",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumGrowthStages",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumImageAngles",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "NumImagesPerGrowthStage",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "NumVariations",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "VariationLikelihood",
                table: "ObjLand",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Colour_11",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ErrorColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MapTooltipCargoColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MapTooltipObjectColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PlayerInfoToolbarColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TimeToolbarColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TooltipColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarPrimaryColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarQuaternaryColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarSecondaryColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TopToolbarTertiaryColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowConstructionColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowMapColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowOptionsColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowPlayerColor",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowTerraFormColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WindowTitlebarColour",
                table: "ObjInterface",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<uint>(
                name: "BuildingSizeFlags",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

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

            migrationBuilder.AddColumn<uint>(
                name: "Colours",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "FarmGrowthStageWithNoProduction",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "FarmIdealSize",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "FarmImagesPerGrowthStage",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "FarmNumStagesOfGrowth",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "FarmTileNumImageAngles",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "Flags",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "MapColour",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MaxNumBuildings",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MinNumBuildings",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MonthlyClosureChance",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "ScaffoldingColour",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ScaffoldingSegmentType",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "TotalOfTypeInScenario",
                table: "ObjIndustry",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjHillShapes",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "HillHeightMapCount",
                table: "ObjHillShapes",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MountainHeightMapCount",
                table: "ObjHillShapes",
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

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "Flags",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

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

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjDock",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "Factor",
                table: "ObjCurrency",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Separator",
                table: "ObjCurrency",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Aggressiveness",
                table: "ObjCompetitor",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "AvailableNamePrefixes",
                table: "ObjCompetitor",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "AvailablePlaystyles",
                table: "ObjCompetitor",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "Competitiveness",
                table: "ObjCompetitor",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "Emotions",
                table: "ObjCompetitor",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "Intelligence",
                table: "ObjCompetitor",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "CargoCategory",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "CargoTransferTime",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MaxNonPremiumDays",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "MaxPremiumRate",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "NumPlatformVariations",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "PaymentFactor",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "PaymentIndex",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "PenaltyRate",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "PremiumDays",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "StationCargoDensity",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "UnitSize",
                table: "ObjCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "AverageNumberOnMap",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "Colours",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "DemolishRatingReduction",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "GeneratorFunction",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "ScaffoldingColour",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ScaffoldingSegmentType",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "SellCostFactor",
                table: "ObjBuilding",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "BaseCostFactor",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ClearHeight",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "DeckDepth",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DisabledTrackFlags",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<byte>(
                name: "Flags",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "HeightCostFactor",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "MaxHeight",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "MaxSpeed",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "PillarSpacing",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "SpanLength",
                table: "ObjBridge",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "AllowedPlaneTypes",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "BuildCostFactor",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "CostIndex",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "DesignedYear",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<uint>(
                name: "LargeTiles",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<sbyte>(
                name: "MaxX",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.AddColumn<sbyte>(
                name: "MaxY",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.AddColumn<sbyte>(
                name: "MinX",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.AddColumn<sbyte>(
                name: "MinY",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ObsoleteYear",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<short>(
                name: "SellCostFactor",
                table: "ObjAirport",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostFactor",
                table: "ObjWater");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjWater");

            migrationBuilder.DropColumn(
                name: "Flags1",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "Flags2",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "ToolId",
                table: "ObjWall");

            migrationBuilder.DropColumn(
                name: "CostFactor",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "DrivingSoundType",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "NumCarComponents",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "NumCompatibleVehicles",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "NumRequiredTrackExtras",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "RackRailType",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "RackSpeed",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "Reliability",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "RunCostFactor",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "RunCostIndex",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "ShipWakeOffset",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "TrackTypeId",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ObjVehicle");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "ClearCostFactor",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "Clearance",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "Colours",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "DemolishRatingReduction",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "NumGrowthStages",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "NumRotations",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "SeasonState",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "ShadowImageOffset",
                table: "ObjTree");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "CargoOffsetBytes",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "CompatibleTrack",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "ManualPower",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "PaintStyle",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "TrackPieces",
                table: "ObjTrackStation");

            migrationBuilder.DropColumn(
                name: "AnimationSpeed",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "NumFrames",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjTrackSignal");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjTrackExtra");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjTrackExtra");

            migrationBuilder.DropColumn(
                name: "PaintStyle",
                table: "ObjTrackExtra");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjTrackExtra");

            migrationBuilder.DropColumn(
                name: "TrackPieces",
                table: "ObjTrackExtra");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "CurveSpeed",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "DisplayOffset",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "StationTrackPieces",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "TrackPieces",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "Tunnel",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "TunnelCostFactor",
                table: "ObjTrack");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "NumStationaryTicks",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "SpriteHeightNegative",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "SpriteHeightPositive",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "SpriteWidth",
                table: "ObjSteam");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "ObjSound");

            migrationBuilder.DropColumn(
                name: "Offset",
                table: "ObjSound");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "CargoTypeId",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "CompatibleRoadObjectCount",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "PaintStyle",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "RoadPieces",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjRoadStation");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjRoadExtra");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjRoadExtra");

            migrationBuilder.DropColumn(
                name: "PaintStyle",
                table: "ObjRoadExtra");

            migrationBuilder.DropColumn(
                name: "RoadPieces",
                table: "ObjRoadExtra");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjRoadExtra");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "DisplayOffset",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "MaxSpeed",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "PaintStyle",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "RoadPieces",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "TargetTownSize",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "Tunnel",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "TunnelCostFactor",
                table: "ObjRoad");

            migrationBuilder.DropColumn(
                name: "AnimationSpeed",
                table: "ObjLevelCrossing");

            migrationBuilder.DropColumn(
                name: "ClosedFrames",
                table: "ObjLevelCrossing");

            migrationBuilder.DropColumn(
                name: "ClosingFrames",
                table: "ObjLevelCrossing");

            migrationBuilder.DropColumn(
                name: "CostFactor",
                table: "ObjLevelCrossing");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjLevelCrossing");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjLevelCrossing");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjLevelCrossing");

            migrationBuilder.DropColumn(
                name: "CliffEdgeHeader1",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "CliffEdgeHeader2",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "CostFactor",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "DistributionPattern",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "NumGrowthStages",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "NumImageAngles",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "NumImagesPerGrowthStage",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "NumVariations",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "VariationLikelihood",
                table: "ObjLand");

            migrationBuilder.DropColumn(
                name: "Colour_11",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "ErrorColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "MapTooltipCargoColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "MapTooltipObjectColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "PlayerInfoToolbarColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "TimeToolbarColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "TooltipColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "TopToolbarPrimaryColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "TopToolbarQuaternaryColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "TopToolbarSecondaryColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "TopToolbarTertiaryColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "WindowColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "WindowConstructionColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "WindowMapColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "WindowOptionsColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "WindowPlayerColor",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "WindowTerraFormColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "WindowTitlebarColour",
                table: "ObjInterface");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BuildingSizeFlags",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BuildingWall",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "BuildingWallEntrance",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "Colours",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "FarmGrowthStageWithNoProduction",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "FarmIdealSize",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "FarmImagesPerGrowthStage",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "FarmNumStagesOfGrowth",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "FarmTileNumImageAngles",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "MapColour",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "MaxNumBuildings",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "MinNumBuildings",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "MonthlyClosureChance",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "ScaffoldingColour",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "ScaffoldingSegmentType",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "TotalOfTypeInScenario",
                table: "ObjIndustry");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjHillShapes");

            migrationBuilder.DropColumn(
                name: "HillHeightMapCount",
                table: "ObjHillShapes");

            migrationBuilder.DropColumn(
                name: "MountainHeightMapCount",
                table: "ObjHillShapes");

            migrationBuilder.DropColumn(
                name: "BoatPositionX",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "BoatPositionY",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "NumBuildingPartAnimations",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "NumBuildingVariationParts",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjDock");

            migrationBuilder.DropColumn(
                name: "Factor",
                table: "ObjCurrency");

            migrationBuilder.DropColumn(
                name: "Separator",
                table: "ObjCurrency");

            migrationBuilder.DropColumn(
                name: "Aggressiveness",
                table: "ObjCompetitor");

            migrationBuilder.DropColumn(
                name: "AvailableNamePrefixes",
                table: "ObjCompetitor");

            migrationBuilder.DropColumn(
                name: "AvailablePlaystyles",
                table: "ObjCompetitor");

            migrationBuilder.DropColumn(
                name: "Competitiveness",
                table: "ObjCompetitor");

            migrationBuilder.DropColumn(
                name: "Emotions",
                table: "ObjCompetitor");

            migrationBuilder.DropColumn(
                name: "Intelligence",
                table: "ObjCompetitor");

            migrationBuilder.DropColumn(
                name: "CargoCategory",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "CargoTransferTime",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "MaxNonPremiumDays",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "MaxPremiumRate",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "NumPlatformVariations",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "PaymentFactor",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "PaymentIndex",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "PenaltyRate",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "PremiumDays",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "StationCargoDensity",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "UnitSize",
                table: "ObjCargo");

            migrationBuilder.DropColumn(
                name: "AverageNumberOnMap",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "Colours",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "DemolishRatingReduction",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "GeneratorFunction",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ScaffoldingColour",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "ScaffoldingSegmentType",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjBuilding");

            migrationBuilder.DropColumn(
                name: "BaseCostFactor",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "ClearHeight",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "DeckDepth",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "DisabledTrackFlags",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "HeightCostFactor",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "MaxHeight",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "MaxSpeed",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "PillarSpacing",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "SpanLength",
                table: "ObjBridge");

            migrationBuilder.DropColumn(
                name: "AllowedPlaneTypes",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "BuildCostFactor",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "CostIndex",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "DesignedYear",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "LargeTiles",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "MaxX",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "MaxY",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "MinX",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "MinY",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "ObsoleteYear",
                table: "ObjAirport");

            migrationBuilder.DropColumn(
                name: "SellCostFactor",
                table: "ObjAirport");
        }
    }
}

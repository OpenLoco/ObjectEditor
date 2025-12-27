using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class SubObjectAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.AddColumn<short>(
			name: "CostFactor",
			table: "ObjWater",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjWater",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags1",
			table: "ObjWall",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags2",
			table: "ObjWall",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Height",
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

		_ = migrationBuilder.AddColumn<short>(
			name: "CostFactor",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "DrivingSoundType",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Mode",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumCarComponents",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumCompatibleVehicles",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumRequiredTrackExtras",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Power",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "RackRailType",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<short>(
			name: "RackSpeed",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Reliability",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "RunCostFactor",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "RunCostIndex",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ShipWakeOffset",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "Speed",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TrackTypeId",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Type",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Weight",
			table: "ObjVehicle",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "ClearCostFactor",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Clearance",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "Colours",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "DemolishRatingReduction",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Height",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumGrowthStages",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumRotations",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "Rating",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Season",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SeasonState",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ShadowImageOffset",
			table: "ObjTree",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<string>(
			name: "CargoOffsetBytes",
			table: "ObjTrackStation",
			type: "TEXT",
			nullable: false,
			defaultValue: "[]");

		_ = migrationBuilder.AddColumn<string>(
			name: "CompatibleTrack",
			table: "ObjTrackStation",
			type: "TEXT",
			nullable: false,
			defaultValue: "[]");

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Height",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<string>(
			name: "ManualPower",
			table: "ObjTrackStation",
			type: "TEXT",
			nullable: false,
			defaultValue: "[]");

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PaintStyle",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "TrackPieces",
			table: "ObjTrackStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "AnimationSpeed",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumFrames",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjTrackSignal",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjTrackExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjTrackExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PaintStyle",
			table: "ObjTrackExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjTrackExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "TrackPieces",
			table: "ObjTrackExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "CurveSpeed",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "DisplayOffset",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "StationTrackPieces",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "TrackPieces",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "Tunnel",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<short>(
			name: "TunnelCostFactor",
			table: "ObjTrack",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjSteam",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumStationaryTicks",
			table: "ObjSteam",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SpriteHeightNegative",
			table: "ObjSteam",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SpriteHeightPositive",
			table: "ObjSteam",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SpriteWidth",
			table: "ObjSteam",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "Length",
			table: "ObjSound",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<int>(
			name: "Offset",
			table: "ObjSound",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "CargoTypeId",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CompatibleRoadObjectCount",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Height",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PaintStyle",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "RoadPieces",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjRoadStation",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjRoadExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjRoadExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PaintStyle",
			table: "ObjRoadExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "RoadPieces",
			table: "ObjRoadExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjRoadExtra",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "DisplayOffset",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "MaxSpeed",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PaintStyle",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "RoadPieces",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TargetTownSize",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ulong>(
			name: "Tunnel",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0ul);

		_ = migrationBuilder.AddColumn<short>(
			name: "TunnelCostFactor",
			table: "ObjRoad",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "AnimationSpeed",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ClosedFrames",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ClosingFrames",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "CostFactor",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjLevelCrossing",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

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

		_ = migrationBuilder.AddColumn<short>(
			name: "CostFactor",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "DistributionPattern",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumGrowthStages",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumImageAngles",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "NumImagesPerGrowthStage",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumVariations",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "VariationLikelihood",
			table: "ObjLand",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Colour_11",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ErrorColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MapTooltipCargoColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MapTooltipObjectColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PlayerInfoToolbarColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TimeToolbarColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TooltipColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TopToolbarPrimaryColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TopToolbarQuaternaryColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TopToolbarSecondaryColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TopToolbarTertiaryColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WindowColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WindowConstructionColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WindowMapColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WindowOptionsColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WindowPlayerColor",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WindowTerraFormColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "WindowTitlebarColour",
			table: "ObjInterface",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "BuildingSizeFlags",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

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

		_ = migrationBuilder.AddColumn<uint>(
			name: "Colours",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "FarmGrowthStageWithNoProduction",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "FarmIdealSize",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "FarmImagesPerGrowthStage",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "FarmNumStagesOfGrowth",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "FarmTileNumImageAngles",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "Flags",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MapColour",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MaxNumBuildings",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MinNumBuildings",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MonthlyClosureChance",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ScaffoldingColour",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ScaffoldingSegmentType",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "TotalOfTypeInScenario",
			table: "ObjIndustry",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjHillShapes",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "HillHeightMapCount",
			table: "ObjHillShapes",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MountainHeightMapCount",
			table: "ObjHillShapes",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BoatPositionX",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BoatPositionY",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "Flags",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumBuildingPartAnimations",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumBuildingVariationParts",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjDock",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Factor",
			table: "ObjCurrency",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Separator",
			table: "ObjCurrency",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Aggressiveness",
			table: "ObjCompetitor",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "AvailableNamePrefixes",
			table: "ObjCompetitor",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<uint>(
			name: "AvailablePlaystyles",
			table: "ObjCompetitor",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Competitiveness",
			table: "ObjCompetitor",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "Emotions",
			table: "ObjCompetitor",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Intelligence",
			table: "ObjCompetitor",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "CargoCategory",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "CargoTransferTime",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MaxNonPremiumDays",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "MaxPremiumRate",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "NumPlatformVariations",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "PaymentFactor",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PaymentIndex",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "PenaltyRate",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PremiumDays",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "StationCargoDensity",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "UnitSize",
			table: "ObjCargo",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "AverageNumberOnMap",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "Colours",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "DemolishRatingReduction",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "GeneratorFunction",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ScaffoldingColour",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "ScaffoldingSegmentType",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "SellCostFactor",
			table: "ObjBuilding",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BaseCostFactor",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ClearHeight",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "DeckDepth",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DisabledTrackFlags",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "Flags",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "HeightCostFactor",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "MaxHeight",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "MaxSpeed",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "PillarSpacing",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "SpanLength",
			table: "ObjBridge",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "AllowedPlaneTypes",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "BuildCostFactor",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);

		_ = migrationBuilder.AddColumn<byte>(
			name: "CostIndex",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (byte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "DesignedYear",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<uint>(
			name: "LargeTiles",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0u);

		_ = migrationBuilder.AddColumn<sbyte>(
			name: "MaxX",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (sbyte)0);

		_ = migrationBuilder.AddColumn<sbyte>(
			name: "MaxY",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (sbyte)0);

		_ = migrationBuilder.AddColumn<sbyte>(
			name: "MinX",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (sbyte)0);

		_ = migrationBuilder.AddColumn<sbyte>(
			name: "MinY",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (sbyte)0);

		_ = migrationBuilder.AddColumn<ushort>(
			name: "ObsoleteYear",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (ushort)0);

		_ = migrationBuilder.AddColumn<short>(
			name: "SellCostFactor",
			table: "ObjAirport",
			type: "INTEGER",
			nullable: false,
			defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropColumn(
			name: "CostFactor",
			table: "ObjWater");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjWater");

		_ = migrationBuilder.DropColumn(
			name: "Flags1",
			table: "ObjWall");

		_ = migrationBuilder.DropColumn(
			name: "Flags2",
			table: "ObjWall");

		_ = migrationBuilder.DropColumn(
			name: "Height",
			table: "ObjWall");

		_ = migrationBuilder.DropColumn(
			name: "ToolId",
			table: "ObjWall");

		_ = migrationBuilder.DropColumn(
			name: "CostFactor",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "DrivingSoundType",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "Mode",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "NumCarComponents",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "NumCompatibleVehicles",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "NumRequiredTrackExtras",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "Power",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "RackRailType",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "RackSpeed",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "Reliability",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "RunCostFactor",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "RunCostIndex",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "ShipWakeOffset",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "Speed",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "TrackTypeId",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "Type",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "Weight",
			table: "ObjVehicle");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "ClearCostFactor",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "Clearance",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "Colours",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "DemolishRatingReduction",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "Height",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "NumGrowthStages",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "NumRotations",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "Rating",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "Season",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "SeasonState",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "ShadowImageOffset",
			table: "ObjTree");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "CargoOffsetBytes",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "CompatibleTrack",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "Height",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "ManualPower",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "PaintStyle",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "TrackPieces",
			table: "ObjTrackStation");

		_ = migrationBuilder.DropColumn(
			name: "AnimationSpeed",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "NumFrames",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjTrackSignal");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropColumn(
			name: "PaintStyle",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropColumn(
			name: "TrackPieces",
			table: "ObjTrackExtra");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "CurveSpeed",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "DisplayOffset",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "StationTrackPieces",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "TrackPieces",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "Tunnel",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "TunnelCostFactor",
			table: "ObjTrack");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjSteam");

		_ = migrationBuilder.DropColumn(
			name: "NumStationaryTicks",
			table: "ObjSteam");

		_ = migrationBuilder.DropColumn(
			name: "SpriteHeightNegative",
			table: "ObjSteam");

		_ = migrationBuilder.DropColumn(
			name: "SpriteHeightPositive",
			table: "ObjSteam");

		_ = migrationBuilder.DropColumn(
			name: "SpriteWidth",
			table: "ObjSteam");

		_ = migrationBuilder.DropColumn(
			name: "Length",
			table: "ObjSound");

		_ = migrationBuilder.DropColumn(
			name: "Offset",
			table: "ObjSound");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "CargoTypeId",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "CompatibleRoadObjectCount",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "Height",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "PaintStyle",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "RoadPieces",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjRoadStation");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropColumn(
			name: "PaintStyle",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropColumn(
			name: "RoadPieces",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjRoadExtra");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "DisplayOffset",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "MaxSpeed",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "PaintStyle",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "RoadPieces",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "TargetTownSize",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "Tunnel",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "TunnelCostFactor",
			table: "ObjRoad");

		_ = migrationBuilder.DropColumn(
			name: "AnimationSpeed",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "ClosedFrames",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "ClosingFrames",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "CostFactor",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjLevelCrossing");

		_ = migrationBuilder.DropColumn(
			name: "CliffEdgeHeader1",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "CliffEdgeHeader2",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "CostFactor",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "DistributionPattern",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "NumGrowthStages",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "NumImageAngles",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "NumImagesPerGrowthStage",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "NumVariations",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "VariationLikelihood",
			table: "ObjLand");

		_ = migrationBuilder.DropColumn(
			name: "Colour_11",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "ErrorColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "MapTooltipCargoColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "MapTooltipObjectColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "PlayerInfoToolbarColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "TimeToolbarColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "TooltipColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "TopToolbarPrimaryColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "TopToolbarQuaternaryColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "TopToolbarSecondaryColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "TopToolbarTertiaryColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "WindowColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "WindowConstructionColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "WindowMapColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "WindowOptionsColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "WindowPlayerColor",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "WindowTerraFormColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "WindowTitlebarColour",
			table: "ObjInterface");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "BuildingSizeFlags",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "BuildingWall",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "BuildingWallEntrance",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "Colours",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "FarmGrowthStageWithNoProduction",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "FarmIdealSize",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "FarmImagesPerGrowthStage",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "FarmNumStagesOfGrowth",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "FarmTileNumImageAngles",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "MapColour",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "MaxNumBuildings",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "MinNumBuildings",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "MonthlyClosureChance",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "ScaffoldingColour",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "ScaffoldingSegmentType",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "TotalOfTypeInScenario",
			table: "ObjIndustry");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropColumn(
			name: "HillHeightMapCount",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropColumn(
			name: "MountainHeightMapCount",
			table: "ObjHillShapes");

		_ = migrationBuilder.DropColumn(
			name: "BoatPositionX",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "BoatPositionY",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "NumBuildingPartAnimations",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "NumBuildingVariationParts",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjDock");

		_ = migrationBuilder.DropColumn(
			name: "Factor",
			table: "ObjCurrency");

		_ = migrationBuilder.DropColumn(
			name: "Separator",
			table: "ObjCurrency");

		_ = migrationBuilder.DropColumn(
			name: "Aggressiveness",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropColumn(
			name: "AvailableNamePrefixes",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropColumn(
			name: "AvailablePlaystyles",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropColumn(
			name: "Competitiveness",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropColumn(
			name: "Emotions",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropColumn(
			name: "Intelligence",
			table: "ObjCompetitor");

		_ = migrationBuilder.DropColumn(
			name: "CargoCategory",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "CargoTransferTime",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "MaxNonPremiumDays",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "MaxPremiumRate",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "NumPlatformVariations",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "PaymentFactor",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "PaymentIndex",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "PenaltyRate",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "PremiumDays",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "StationCargoDensity",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "UnitSize",
			table: "ObjCargo");

		_ = migrationBuilder.DropColumn(
			name: "AverageNumberOnMap",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "Colours",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "DemolishRatingReduction",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "GeneratorFunction",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "ScaffoldingColour",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "ScaffoldingSegmentType",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjBuilding");

		_ = migrationBuilder.DropColumn(
			name: "BaseCostFactor",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "ClearHeight",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "DeckDepth",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "DisabledTrackFlags",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "Flags",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "HeightCostFactor",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "MaxHeight",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "MaxSpeed",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "PillarSpacing",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "SpanLength",
			table: "ObjBridge");

		_ = migrationBuilder.DropColumn(
			name: "AllowedPlaneTypes",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "BuildCostFactor",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "CostIndex",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "DesignedYear",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "LargeTiles",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "MaxX",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "MaxY",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "MinX",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "MinY",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "ObsoleteYear",
			table: "ObjAirport");

		_ = migrationBuilder.DropColumn(
			name: "SellCostFactor",
			table: "ObjAirport");
        }
    }

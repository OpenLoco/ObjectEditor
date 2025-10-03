using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.BridgeObjectLoader;

namespace Dat.Loaders;

public abstract class BridgeObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int _03PadSize = 3;
		public const int MaxNumTrackMods = 7;
		public const int MaxNumRoadMods = 7;
	}

	public static ObjectType ObjectType => ObjectType.Bridge;
	public static DatObjectType DatObjectType => DatObjectType.Bridge;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new BridgeObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.Flags = ((DatBridgeObjectFlags)br.ReadByte()).Convert();
			model.var_03 = br.ReadByte();
			model.ClearHeight = br.ReadUInt16();
			model.DeckDepth = br.ReadInt16();
			model.SpanLength = br.ReadByte();
			model.PillarSpacing = ((DatSupportPillarSpacing)br.ReadByte()).Convert(); // This is a bitfield, see https
			model.MaxSpeed = (Speed16)br.ReadInt16();
			model.MaxHeight = (MicroZ)br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.BaseCostFactor = br.ReadInt16();
			model.HeightCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.DisabledTrackFlags = ((DatBridgeDisabledTrackFlags)br.ReadUInt16()).Convert();
			br.SkipImageId(); // Image offset, not part of object definition
			var compatibleTrackCount = br.ReadByte();
			br.SkipObjectId(Constants.MaxNumTrackMods);  // Placeholder for track mods, not part of object definition
			var compatibleRoadCount = br.ReadByte();
			br.SkipObjectId(Constants.MaxNumRoadMods);  // Placeholder for road mods, not part of object definition
			model.DesignedYear = br.ReadUInt16();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			model.CompatibleTrackObjects = br.ReadS5HeaderList(compatibleTrackCount);
			model.CompatibleRoadObjects = br.ReadS5HeaderList(compatibleRoadCount);

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as BridgeObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId();// Name offset, not part of object definition
			bw.Write((uint8_t)model.Flags);
			bw.Write(model.var_03);
			bw.Write(model.ClearHeight);
			bw.Write(model.DeckDepth);
			bw.Write(model.SpanLength);
			bw.Write((uint8_t)model.PillarSpacing); // This is a bitfield, see https://
			bw.Write(model.MaxSpeed);
			bw.Write(model.MaxHeight);
			bw.Write(model.CostIndex);
			bw.Write(model.BaseCostFactor);
			bw.Write(model.HeightCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write((uint16_t)model.DisabledTrackFlags.Convert());
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write((uint8_t)model.CompatibleTrackObjects.Count);
			bw.WriteEmptyObjectId(Constants.MaxNumTrackMods); // Placeholder for track mods, not part of object definition
			bw.Write((uint8_t)model.CompatibleRoadObjects.Count);
			bw.WriteEmptyObjectId(Constants.MaxNumRoadMods); // Placeholder for road mods, not part of object definition
			bw.Write(model.DesignedYear);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5HeaderList(model.CompatibleTrackObjects);
			bw.WriteS5HeaderList(model.CompatibleRoadObjects);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	public enum DatSupportPillarSpacing : uint8_t
	{
		Tile0A = 1 << 0,
		Tile0B = 1 << 1,
		Tile1A = 1 << 2,
		Tile1B = 1 << 3,
		Tile2A = 1 << 4,
		Tile2B = 1 << 5,
		Tile3A = 1 << 6,
		Tile3B = 1 << 7,
	}

	[Flags]
	internal enum DatBridgeDisabledTrackFlags : uint16_t
	{
		None = 0,
		Slope = 1 << 0,
		SteepSlope = 1 << 1,
		CurveSlope = 1 << 2,
		Diagonal = 1 << 3,
		VerySmallCurve = 1 << 4,
		SmallCurve = 1 << 5,
		Curve = 1 << 6,
		LargeCurve = 1 << 7,
		SBendCurve = 1 << 8,
		OneSided = 1 << 9,
		StartsAtHalfHeight = 1 << 10, // Not used. From RCT2
		Junction = 1 << 11,
	}

	[Flags]
	internal enum DatBridgeObjectFlags : uint8_t
	{
		None = 0,
		HasRoof = 1 << 0,
	}
}

static class BridgeFlagsConverter
{
	public static DatBridgeObjectFlags Convert(this BridgeObjectFlags bridgeObjectFlags)
		=> (DatBridgeObjectFlags)bridgeObjectFlags;

	public static BridgeObjectFlags Convert(this DatBridgeObjectFlags datBridgeObjectFlags)
		=> (BridgeObjectFlags)datBridgeObjectFlags;
}

static class BridgeDisabledFlagsConverter
{
	public static DatBridgeDisabledTrackFlags Convert(this BridgeDisabledTrackFlags bridgeDisabledTrackFlags)
		=> (DatBridgeDisabledTrackFlags)bridgeDisabledTrackFlags;

	public static BridgeDisabledTrackFlags Convert(this DatBridgeDisabledTrackFlags datBridgeDisabledTrackFlags)
		=> (BridgeDisabledTrackFlags)datBridgeDisabledTrackFlags;
}

static class SupportPillarSpacingFlagsConverter
{
	public static DatSupportPillarSpacing Convert(this SupportPillarSpacing supportPillarSpacing)
		=> (DatSupportPillarSpacing)supportPillarSpacing;

	public static SupportPillarSpacing Convert(this DatSupportPillarSpacing datSupportPillarSpacing)
		=> (SupportPillarSpacing)datSupportPillarSpacing;
}

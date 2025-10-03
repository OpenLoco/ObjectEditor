using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.DockObjectLoader;

namespace Dat.Loaders;

public abstract class DockObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int BuildingVariationCount = 32;
		public const int BuildingHeightCount = 4;
		public const int BuildingAnimationCount = 2;
	}

	public static class StructSizes
	{
		public const int Dat = 0x28;
	}

	public static ObjectType ObjectType => ObjectType.Dock;
	public static DatObjectType DatObjectType => DatObjectType.Dock;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new DockObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.var_07 = br.ReadByte(); // probably padding
			br.SkipImageId(); // Image, not part of object definition
			br.SkipImageId(); // UnkImage, not part of object definition
			model.Flags = ((DatDockObjectFlags)br.ReadUInt16()).Convert();
			var numBuildingParts = br.ReadByte();
			var numBuildingVariations = br.ReadByte();
			br.SkipPointer(); // BuildingPartHeights
			br.SkipPointer(); // BuildingPartAnimations
			br.SkipPointer(); // BuildingVariationParts
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			model.BoatPosition = new Pos2
			{
				X = br.ReadInt16(),
				Y = br.ReadInt16()
			};

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			model.BuildingComponents.BuildingHeights = br.ReadBuildingHeights(numBuildingParts);
			model.BuildingComponents.BuildingAnimations = br.ReadBuildingAnimations(numBuildingParts);
			model.BuildingComponents.BuildingVariations = br.ReadBuildingVariations(numBuildingVariations);

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
		var model = (DockObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.var_07); // probably padding
			bw.WriteEmptyImageId(); // Image, not part of object definition
			bw.WriteEmptyImageId(); // UnkImage, not part of object definition
			bw.Write((uint16_t)model.Flags.Convert());
			bw.Write((uint8_t)model.BuildingComponents.BuildingAnimations.Count);
			bw.Write((uint8_t)model.BuildingComponents.BuildingVariations.Count);
			bw.WriteEmptyPointer(); // BuildingPartHeights
			bw.WriteEmptyPointer(); // BuildingPartAnimations
			bw.WriteEmptyPointer(); // BuildingVariationParts
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.Write(model.BoatPosition.X);
			bw.Write(model.BoatPosition.Y);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	private static void SaveVariable(DockObject model, LocoBinaryWriter bw)
	{
		bw.Write(model.BuildingComponents.BuildingHeights);
		bw.Write(model.BuildingComponents.BuildingAnimations);
		bw.Write(model.BuildingComponents.BuildingVariations);
	}

	[Flags]
	internal enum DatDockObjectFlags : uint16_t
	{
		None = 0,
		HasShadows = 1 << 0,
	}
}

internal static class DockObjectFlagsConverter
{
	public static DockObjectFlags Convert(this DatDockObjectFlags datDockObjectFlags)
		=> (DockObjectFlags)datDockObjectFlags;

	public static DatDockObjectFlags Convert(this DockObjectFlags dockObjectFlags)
		=> (DatDockObjectFlags)dockObjectFlags;
}

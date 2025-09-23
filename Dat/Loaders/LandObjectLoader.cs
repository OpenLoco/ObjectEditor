using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.LandObjectLoader;

namespace Dat.Loaders;

public abstract class LandObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x1E;
	}

	public static ObjectType ObjectType => ObjectType.Land;
	public static DatObjectType DatObjectType => DatObjectType.Land;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new LandObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.CostIndex = br.ReadByte();
			model.NumGrowthStages = br.ReadByte();
			model.NumImageAngles = br.ReadByte();
			model.Flags = ((DatLandObjectFlags)br.ReadByte()).Convert();
			br.SkipObjectId(); // CliffEdgeHeader1, not part of object definition
			br.SkipObjectId(); // CliffEdgeHeader2, not part of object
			model.CostFactor = br.ReadInt16();
			br.SkipImageId(); // Image offset, not part of object definition
			model.NumImagesPerGrowthStage = br.ReadUInt32();
			br.SkipImageId(); // CliffEdgeImage, not part of object definition
			br.SkipImageId(); // MapPixelImage, not part of object definition
			model.DistributionPattern = br.ReadByte();
			model.NumVariations = br.ReadByte();
			model.VariationLikelihood = br.ReadByte();
			model.pad_1D = br.ReadByte();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			model.CliffEdgeHeader1 = br.ReadS5Header();
			if (model.Flags.HasFlag(LandObjectFlags.HasExtraCliffEdge))
			{
				model.CliffEdgeHeader2 = br.ReadS5Header();
			}

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
		var model = (LandObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.CostIndex);
			bw.Write(model.NumGrowthStages);
			bw.Write(model.NumImageAngles);
			bw.Write((uint8_t)model.Flags.Convert());
			bw.WriteEmptyObjectId(); // CliffEdgeHeader1, not part of object definition
			bw.WriteEmptyObjectId(); // CliffEdgeHeader2, not part of object definition
			bw.Write(model.CostFactor);
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write(model.NumImagesPerGrowthStage);
			bw.WriteEmptyImageId(); // CliffEdgeImage, not part of object definition
			bw.WriteEmptyImageId(); // MapPixelImage, not part of object definition
			bw.Write(model.DistributionPattern);
			bw.Write(model.NumVariations);
			bw.Write(model.VariationLikelihood);
			bw.Write(model.pad_1D);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5Header(model.CliffEdgeHeader1);
			if (model.Flags.HasFlag(LandObjectFlags.HasExtraCliffEdge))
			{
				ArgumentNullException.ThrowIfNull(model.CliffEdgeHeader2); // cannot have flag set but no extra header
				bw.WriteS5Header(model.CliffEdgeHeader2);
			}

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatLandObjectFlags : uint8_t
	{
		None = 0,
		unk_00 = 1 << 0,
		HasUnkObjectHeader = 1 << 1,
		IsDesert = 1 << 2,
		NoTrees = 1 << 3,
		unk_04 = 1 << 4,
		unk_05 = 1 << 5,
	}
}

internal static class LandObjectFlagsConverter
{
	public static LandObjectFlags Convert(this DatLandObjectFlags datLandObjectFlags)
		=> (LandObjectFlags)datLandObjectFlags;

	public static DatLandObjectFlags Convert(this LandObjectFlags landObjectFlags)
		=> (DatLandObjectFlags)landObjectFlags;
}

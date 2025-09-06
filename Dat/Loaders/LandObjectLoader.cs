using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
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

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new LandObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

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
			br.SkipByte(); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Land), null);

			// variable
			model.CliffEdgeHeader = br.ReadS5Header();
			if (model.Flags.HasFlag(LandObjectFlags.HasUnkObjectHeader))
			{
				model.UnkObjectHeader = br.ReadS5Header();
			}

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Land, model, stringTable, imageTable);
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
			bw.Write((uint8_t)0); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5Header(model.CliffEdgeHeader);
			if (model.Flags.HasFlag(LandObjectFlags.HasUnkObjectHeader))
			{
				ArgumentNullException.ThrowIfNull(model.UnkObjectHeader); // cannot have flag set but no unk header
				bw.WriteS5Header(model.UnkObjectHeader);
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

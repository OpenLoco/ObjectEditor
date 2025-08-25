using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.TrackSignalObjectLoader;

namespace Dat.Loaders;

public abstract class TrackSignalObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int ModsLength = 7;
	}

	internal static class StructSizes
	{
		public const int Dat = 0x1E;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TrackSignalObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.Flags = ((DatTrackSignalObjectFlags)br.ReadUInt16()).Convert();
			model.AnimationSpeed = br.ReadByte();
			model.NumFrames = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.var_0B = br.ReadByte();
			br.SkipStringId(); // Description, not part of object definition
			br.SkipImageId(); // BaseImageOffset, not part of object definition
			var compatibleTrackCount = br.ReadByte();
			br.SkipObjectId(Constants.ModsLength); // Mods, not part of object definition
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.TrackSignal), null);

			// variable
			model.CompatibleTrackObjects = br.ReadS5HeaderList(compatibleTrackCount);

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.TrackSignal, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as TrackSignalObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId();// Name offset, not part of object definition
			bw.Write((uint16_t)model.Flags);
			bw.Write(model.AnimationSpeed);
			bw.Write(model.NumFrames);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.var_0B);
			bw.Write((string_id)0); // Description, not part of object definition
			bw.Write((image_id)0); // BaseImageOffset, not part of object definition
			bw.Write((uint8_t)model.CompatibleTrackObjects.Count);
			bw.WriteEmptyObjectId(Constants.ModsLength); // Mods, not part of object definition
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5HeaderList(model.CompatibleTrackObjects);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatTrackSignalObjectFlags : uint16_t
	{
		None = 0 << 0,
		IsLeft = 1 << 0,
		HasLights = 1 << 1,
		unk_02 = 1 << 2,
	}

}

static class TrackSignalFlagsConverter
{
	public static DatTrackSignalObjectFlags Convert(this TrackSignalObjectFlags trackSignalFlags)
		=> (DatTrackSignalObjectFlags)trackSignalFlags;

	public static TrackSignalObjectFlags Convert(this DatTrackSignalObjectFlags datTrackSignalFlags)
		=> (TrackSignalObjectFlags)datTrackSignalFlags;
}


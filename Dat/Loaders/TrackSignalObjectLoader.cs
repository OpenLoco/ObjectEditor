using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.TrackSignalObjectLoader;

namespace Dat.Loaders;

public abstract class TrackSignalObjectLoader : IDatObjectLoader
{
	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			var model = new TrackSignalObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.BaseStream.Seek(0x02, SeekOrigin.Begin); // Skip name offset
			model.Flags = ((DatTrackSignalObjectFlags)br.ReadUInt16()).Convert();
			model.AnimationSpeed = br.ReadByte();
			model.NumFrames = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.var_0B = br.ReadByte();
			_ = br.BaseStream.Seek(2, SeekOrigin.Current); // Skip description offset
			_ = br.BaseStream.Seek(2, SeekOrigin.Current); // Skip image offset
			var compatibleTrackCount = br.ReadByte();
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();

			// move to string table
			var structSize = ObjectAttributes.StructSize(DatObjectType.TrackSignal);
			_ = br.BaseStream.Seek(structSize, SeekOrigin.Begin);

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.RoadExtra), null);

			// variable
			model.CompatibleTrackObjects = SawyerStreamReader.LoadVariableCountS5HeadersStream(stream, compatibleTrackCount);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.RoadExtra, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var model = obj.Object as TrackSignalObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId();// Name offset, not part of object definition
			bw.Write((uint16_t)model.Flags);
			bw.Write(model.AnimationSpeed);
			bw.Write(model.NumFrames);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write((string_id)0); // Description, not part of object definition
			bw.Write((image_id)0); // BaseImageOffset, not part of object definition
			bw.Write((uint8_t)model.CompatibleTrackObjects.Count);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			bw.WriteS5HeaderList(model.CompatibleTrackObjects);

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
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


using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.RoadExtra;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Types;
using System.IO;

namespace Dat.Objects;

public static class DatTrackSignalObject
{
	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			var model = new TrackSignalObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.BaseStream.Seek(2, SeekOrigin.Current); // Skip name offset
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
			model.CompatibleTrackObjects = [.. SawyerStreamReader
				.LoadVariableCountS5HeadersStream(stream, compatibleTrackCount)
				.Select(x => new ObjectModelHeader(x.Name, x.Checksum, x.ObjectType.Convert(), x.ObjectSource.Convert()))];

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.RoadExtra, model, stringTable, imageTable);
		}
	}

	public static MemoryStream Save(MemoryStream ms, LocoObject obj)
	{
		var model = obj.Object as TrackSignalObject;

		using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			bw.Write((string_id)0);// Name offset, not part of object definition
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
			_ = SawyerStreamWriter.WriteStringTableStream(ms, obj.StringTable);

			// variable
			foreach (var cto in model.CompatibleTrackObjects)
			{
				var s5Header = new S5Header(cto.Name, cto.Checksum)
				{
					ObjectType = cto.ObjectType.Convert(),
					ObjectSource = cto.ObjectSource.Convert()
				};
				bw.Write(s5Header.Write());
			}

			// image table
			_ = SawyerStreamWriter.WriteImageTableStream(ms, obj.GraphicsElements);
		}
		return ms;
	}
}

[Flags]
public enum DatTrackSignalObjectFlags : uint16_t
{
	None = 0 << 0,
	IsLeft = 1 << 0,
	HasLights = 1 << 1,
	unk_02 = 1 << 2,
}

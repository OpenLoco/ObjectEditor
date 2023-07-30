using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenLocoTool.Objects;

namespace OpenLocoTool
{
	//public record LocoObject(DatFileHeader datHdr, ObjHeader objHdr, object? obj);

	public interface ILocoObject
	{ }

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public ref struct LocoObject //: ILocoObject
	{
		public LocoObject(byte[] data)
		{
			this.data = data;
			DatFileHeader = ref MemoryMarshal.Cast<byte, DatFileHeader>(data[..Constants.DatFileHeaderSize])[0];
			ObjHeader = ref MemoryMarshal.Cast<byte, ObjHeader>(data[Constants.DatFileHeaderSize..(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)])[0];
		}

		public ref DatFileHeader DatFileHeader;
		public ref ObjHeader ObjHeader;

		readonly byte[] data; // this owns the data

		public byte[] ObjectData => data[(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)..];

		//public ref DatFileHeader DatFileHeader
		//	=> ref MemoryMarshal.Cast<byte, DatFileHeader>(data[..Constants.DatFileHeaderSize])[0];

		//public ref ObjHeader ObjHeader
		//	=> ref MemoryMarshal.Cast<byte, ObjHeader>(data[Constants.DatFileHeaderSize..(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)])[0];

		public ref T DataAs<T>() where T : struct
			=> ref MemoryMarshal.Cast<byte, T>(data[(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)..])[0];

		public Type? UnderlyingObjectType()
			=> DatFileHeader.ObjectType switch
			{
				ObjectType.bridge => typeof(BridgeObject),
				ObjectType.building => typeof(BuildingObject),
				ObjectType.cargo => typeof(CargoObject),
				ObjectType.cliffEdge => typeof(CliffEdgeObject),
				ObjectType.climate => typeof(ClimateObject),
				ObjectType.competitor => typeof(CompetitorObject),
				ObjectType.currency => typeof(CurrencyObject),
				ObjectType.dock => typeof(DockObject),
				ObjectType.hillShapes => typeof(HillShapesObject),
				ObjectType.industry => typeof(IndustryObject),
				ObjectType.track => typeof(TrackObject),
				ObjectType.trackSignal => typeof(TrainSignalObject),
				ObjectType.tree => typeof(TreeObject),
				ObjectType.vehicle => typeof(VehicleObject),
				_ => null,
			};

	}
}

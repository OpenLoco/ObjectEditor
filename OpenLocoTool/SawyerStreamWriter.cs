using System.Runtime.InteropServices;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenLocoTool
{
	public class SawyerStreamWriter
	{
		private readonly ILogger Logger;

		public SawyerStreamWriter(ILogger logger)
			=> Logger = logger;

		public void Save(string path, LocoObject locoObject)
		{
			if (locoObject == null)
			{
				throw new ArgumentNullException(nameof(locoObject));
			}

			Logger.Log(LogLevel.Info, $"Writing to {locoObject.datHdr.Name} to {path}");

			//var datHdr = MemoryMarshal.Write(locoObject.datHdr);
			//var objHeader = MemoryMarshal.Write(locoObject.objHdr);
			//var encoded = Encode(locoObject.objHdr.Encoding, WriteObject(locoObject.obj));

			//WriteToFile(datHdr, objHdr, encoded);

		}

		//private ReadOnlySpan<byte> Encode<T>(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		//{
		//	switch (encoding)
		//	{
		//		case SawyerEncoding.uncompressed:
		//			return data;
		//		case SawyerEncoding.runLengthSingle:
		//			return decodeRunLengthSingle(data);
		//		case SawyerEncoding.runLengthMulti:
		//			return decodeRunLengthMulti(decodeRunLengthSingle(data));
		//		case SawyerEncoding.rotate:
		//			return decodeRotate(data);
		//		default:
		//			Logger.Log(LogLevel.Error, "Unknown chunk encoding scheme");
		//			throw new InvalidDataException("Unknown encoding");
		//	}
		//}

		// inverse of SawyerStreamReeader.ReadObject
		//private ReadOnlySpan<byte> WriteObject(ObjectType objClass, object? locoObject)
		//{
		//	if (locoObject == null)
		//	{
		//		throw new ArgumentNullException(nameof(locoObject));
		//	}

		//	var span = new ReadOnlySpan<byte>();
		//	object? obj = objClass switch
		//	{
		//		ObjectType.bridge => MemoryMarshal.Write<BridgeObject>(span, ref ),
		//		ObjectType.building => MemoryMarshal.Write<BuildingObject>(data),
		//		ObjectType.cargo => MemoryMarshal.Write<CargoObject>(data),
		//		ObjectType.cliffEdge => MemoryMarshal.Write<CliffEdgeObject>(data),
		//		ObjectType.climate => MemoryMarshal.Write<ClimateObject>(data),
		//		ObjectType.competitor => MemoryMarshal.Write<CompetitorObject>(data),
		//		ObjectType.currency => MemoryMarshal.Write<CurrencyObject>(data),
		//		ObjectType.dock => MemoryMarshal.Write<DockObject>(data),
		//		ObjectType.hillShapes => MemoryMarshal.Write<HillShapesObject>(data),
		//		ObjectType.industry => MemoryMarshal.Write<IndustryObject>(data),
		//		ObjectType.track => MemoryMarshal.Write<TrackObject>(data),
		//		ObjectType.trackSignal => MemoryMarshal.Write<TrainSignalObject>(data),
		//		ObjectType.tree => MemoryMarshal.Write<TreeObject>(data),
		//		ObjectType.vehicle => MemoryMarshal.Write<VehicleObject>(data),
		//		_ => null,
		//	};
		//}

		public void WriteToFile(ReadOnlySpan<byte> datHeader, ReadOnlySpan<byte> objHeader, ReadOnlySpan<byte> data)
		{
			//var BasePath = @"Q:\Steam\steamapps\common\Locomotion\ObjData";
			var BasePath = @"Q:\Steam\steamapps\common\Locomotion";
			var decoded = File.Create(Path.Combine(BasePath, "decoded.dat")); // todo: change name
			decoded.Write(MemoryMarshal.AsBytes(datHeader));
			decoded.Write(objHeader);
			decoded.Write(data);
			decoded.Flush();
			decoded.Close();
		}
	}
}

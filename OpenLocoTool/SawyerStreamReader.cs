using System.Runtime.InteropServices;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;

namespace OpenLocoTool
{
	public class SawyerStreamReader
	{
		private readonly ILogger Logger;

		public SawyerStreamReader(ILogger logger)
			=> Logger = logger;

		// load file
		public (DatFileHeader datHdr, ObjHeader objHdr, object? obj) Load(string path)
		{
			if (!File.Exists(path))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {path}");
				return default;
			}

			Logger.Log(LogLevel.Info, $"Decoding {path}");
			var bytes = File.ReadAllBytes(path);
			var loaded = Load(bytes);

			// validate checksum
			// only valid for S5 files, not individual dat files
			//var valid = validateChecksum(bytes);
			//dataDump.Add(valid.ToString());

			return loaded;
		}

		//public bool validateChecksum(ReadOnlySpan<byte> data)
		//{
		//	//int ptr = 0;
		//	var valid = false;
		//	if (data.Length >= 4)
		//	{
		//		// Read checksum
		//		var checksum = BitConverter.ToUInt32(data[^4..^0]);

		//		// Calculate checksum
		//		uint actualChecksum = 0;
		//		var bufSize = 2048;
		//		var buffer = new byte[bufSize];
		//		for (var i = 0; i < data.Length - 4; i += bufSize)
		//		{
		//			var readLength = Math.Min(bufSize, data.Length - 4 - i);
		//			data.Slice(i, readLength);
		//			for (var j = 0; j < readLength; j++)
		//			{
		//				actualChecksum += buffer[j];
		//			}
		//		}

		//		valid = checksum == actualChecksum;
		//	}

		//	return valid;
		//}

		public const int DatFileHeaderSize = 0x10;
		public const int ObjHeaderSize = 0x05;

		public (DatFileHeader, ObjHeader, object?) Load(ReadOnlySpan<byte> data)
		{
			var datHeader = MemoryMarshal.AsRef<DatFileHeader>(data[..DatFileHeaderSize]);
			var objHeader = MemoryMarshal.AsRef<ObjHeader>(data[DatFileHeaderSize..(DatFileHeaderSize + ObjHeaderSize)]);
			var decoded = Decode(objHeader, data[(DatFileHeaderSize + ObjHeaderSize)..]);

			// call this on Save();
			WriteDecodedObject(data[..DatFileHeaderSize], data[DatFileHeaderSize..(DatFileHeaderSize + ObjHeaderSize)], decoded);

			var obj = ReadObject(decoded, datHeader.ObjectType);
			// parse

			return (datHeader, objHeader, obj);
		}

		public void WriteDecodedObject(ReadOnlySpan<byte> DatHeader, ReadOnlySpan<byte> objHeader, ReadOnlySpan<byte> data)
		{
			//var BasePath = @"Q:\Steam\steamapps\common\Locomotion\ObjData";
			var BasePath = @"Q:\Steam\steamapps\common\Locomotion";
			var decoded = File.Create(Path.Combine(BasePath, "decoded.dat"));
			decoded.Write(MemoryMarshal.AsBytes(DatHeader));
			decoded.Write(objHeader);
			decoded.Write(data);
			decoded.Flush();
			decoded.Close();
		}

		// taken from openloco's SawyerStreamReader::readChunk
		private ReadOnlySpan<byte> Decode(ObjHeader objHeader, ReadOnlySpan<byte> data)
		{
			switch (objHeader.Encoding)
			{
				case SawyerEncoding.uncompressed:
					return data;
				case SawyerEncoding.runLengthSingle:
					return decodeRunLengthSingle(data);
				case SawyerEncoding.runLengthMulti:
					return decodeRunLengthMulti(decodeRunLengthSingle(data));
				case SawyerEncoding.rotate:
					return decodeRotate(data);
				default:
					Logger.Log(LogLevel.Error, "Unknown chunk encoding scheme");
					throw new InvalidDataException("Unknown encoding");
			}
		}

		private object? ReadObject(ReadOnlySpan<byte> data, ObjectType objClass)
		{
			object? obj = objClass switch
			{
				ObjectType.bridge => MemoryMarshal.AsRef<BridgeObject>(data),
				ObjectType.building => MemoryMarshal.AsRef<BuildingObject>(data),
				ObjectType.cargo => MemoryMarshal.AsRef<CargoObject>(data),
				ObjectType.cliffEdge => MemoryMarshal.AsRef<CliffEdgeObject>(data),
				ObjectType.climate => MemoryMarshal.AsRef<ClimateObject>(data),
				ObjectType.competitor => MemoryMarshal.AsRef<CompetitorObject>(data),
				ObjectType.currency => MemoryMarshal.AsRef<CurrencyObject>(data),
				ObjectType.dock => MemoryMarshal.AsRef<DockObject>(data),
				ObjectType.hillShapes => MemoryMarshal.AsRef<HillShapesObject>(data),
				ObjectType.industry => MemoryMarshal.AsRef<IndustryObject>(data),
				ObjectType.track => MemoryMarshal.AsRef<TrackObject>(data),
				ObjectType.trackSignal => MemoryMarshal.AsRef<TrainSignalObject>(data),
				ObjectType.tree => MemoryMarshal.AsRef<TreeObject>(data),
				ObjectType.vehicle => MemoryMarshal.AsRef<VehicleObject>(data),
				_ => null,
			};

			Logger.Info(ReflectionLogger.ToString(obj));
			return obj;
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthSingle
		private Span<byte> decodeRunLengthSingle(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			for (var i = 0; i < data.Length; i++)
			{
				var rleCodeByte = data[i];
				if ((rleCodeByte & 128) > 0)
				{
					i++;
					if (i >= data.Length)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					var copyLen = 257 - rleCodeByte;
					var copyByte = data[i];
					buffer.AddRange(Enumerable.Repeat(copyByte, copyLen));
				}
				else
				{
					if (i + 1 >= data.Length || i + 1 + rleCodeByte + 1 > data.Length)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					var copyLen = rleCodeByte + 1;

					for (var j = 0; j < copyLen; ++j)
					{
						buffer.Add(data[i + 1 + j]);
					}

					i += rleCodeByte + 1;
				}
			}

			// convert to span
			Span<byte> decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthMulti
		private Span<byte> decodeRunLengthMulti(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			for (var i = 0; i < data.Length; i++)
			{
				if (data[i] == 0xFF)
				{
					i++;
					if (i >= data.Length)
					{
						throw new ArgumentException("Invalid RLE run");
					}
					buffer.Add(data[i]);
				}
				else
				{
					var offset = (data[i] >> 3) - 32;
					//assert(offset < 0);
					if ((-offset) > buffer.Count)
					{
						throw new ArgumentException("Invalid RLE run");
					}
					var copySrc = 0 + buffer.Count + offset;
					var copyLen = (data[i] & 7) + 1;

					// Copy it to temp buffer first as we can't copy buffer to itself due to potential
					// realloc in between reserve and push
					//uint8_t copyBuffer[32];
					//assert(copyLen <= sizeof(copyBuffer));
					//std::memcpy(copyBuffer, copySrc, copyLen);
					//buffer.push_back(copyBuffer, copyLen);

					buffer.AddRange(buffer.GetRange(copySrc, copyLen));
				}
			}

			// convert to span
			Span<byte> decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		private Span<byte> decodeRotate(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			byte code = 1;
			for (var i = 0; i < data.Length; i++)
			{
				buffer.Add(ror(data[i], code));
				code = (byte)((code + 2) & 7);
			}

			// convert to span
			Span<byte> decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		private byte ror(byte x, byte shift)
		{
			const byte byteDigits = 8;
			return (byte)((x >> shift) | (x << (byteDigits - shift)));
		}
	}
}

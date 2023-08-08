using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;

namespace OpenLocoTool.DatFileParsing
{
	public class SawyerStreamReader
	{
		private readonly ILogger Logger;

		public SawyerStreamReader(ILogger logger)
			=> Logger = logger;

		// load file
		public ILocoObject Load(string path)
		{
			var (ObjHdr1, ObjHDr2, Data) = LoadFromFile(path);
			var decodedData = Decode(ObjHDr2.Encoding, Data);
			return ParseData(ObjHdr1, ObjHDr2, decodedData);
		}
		public (ObjectHeader ObjHdr1, ObjectHeader2 ObjHdr2, byte[] RawData) LoadFromFile(string path)
		{
			if (!File.Exists(path))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {path}");
				return default;
			}

			Logger.Log(LogLevel.Info, $"Loading {path}");
			Span<byte> data = File.ReadAllBytes(path);

			var objectHeader = (ObjectHeader)ObjectHeader.Read(data[..Constants.DatFileHeaderSize]);
			var objectHeader2 = (ObjectHeader2)ObjectHeader2.Read(data[Constants.DatFileHeaderSize..(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)]);

			return (objectHeader, objectHeader2, data[(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)..].ToArray());
		}

		public static ILocoObject ParseData(ObjectHeader objectHeader, ObjectHeader2 objectHeader2, ReadOnlySpan<byte> data)
		{
			return objectHeader.ObjectType switch
			{
				ObjectType.bridge => MakeLocoObject(objectHeader, objectHeader2, ByteReader.ReadLocoStruct<BridgeObject>(data)),
				//ObjectType.building => MakeLocoObject<BuildingObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.cargo => MakeLocoObject<CargoObject>(datFileHeader, objHeader, objSpan),
				ObjectType.cliffEdge => MakeLocoObject(objectHeader, objectHeader2, ByteReader.ReadLocoStruct<CliffEdgeObject>(data)),
				ObjectType.climate => MakeLocoObject(objectHeader, objectHeader2, ByteReader.ReadLocoStruct<ClimateObject>(data)),
				ObjectType.competitor => MakeLocoObject(objectHeader, objectHeader2, ByteReader.ReadLocoStruct<CompetitorObject>(data)),
				ObjectType.currency => MakeLocoObject(objectHeader, objectHeader2, ByteReader.ReadLocoStruct<CurrencyObject>(data)),
				//ObjectType.dock => MakeLocoObject<DockObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.hillShapes => MakeLocoObject<HillShapesObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.industry => MakeLocoObject<IndustryObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.track => MakeLocoObject<TrackObject>(datFileHeader, objHeader, objSpan),
				ObjectType.signal => MakeLocoObject(objectHeader, objectHeader2, ByteReader.ReadLocoStruct<TrainSignalObject>(data)),
				//ObjectType.tree => MakeLocoObject<TreeObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.vehicle => MakeLocoObject<VehicleObject>(datFileHeader, objHeader, objSpan),
				_ => new LocoObject(objectHeader, objectHeader2, new EmptyObject("<unknown>"))
			};
		}

		private static ILocoObject MakeLocoObject(ObjectHeader datFileHeader, ObjectHeader2 objHeader, ILocoStruct obj)
			=> new LocoObject(datFileHeader, objHeader, obj);

		//private static ILocoObject MakeLocoObject<T>(DatFileHeader datFileHeader, ObjHeader ObjHeader, ReadOnlySpan<byte> objSpan) where T : struct
		//	=> new LocoObject<T>(datFileHeader, ObjHeader, MemoryMarshal.Read<T>(objSpan));

		// taken from openloco's SawyerStreamReader::readChunk
		public byte[] Decode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		{
			switch (encoding)
			{
				case SawyerEncoding.uncompressed:
					return data.ToArray();
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

		// taken from openloco SawyerStreamReader::decodeRunLengthSingle
		private byte[] decodeRunLengthSingle(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			for (var i = 0; i < data.Length; ++i)
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
			var decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthMulti
		private byte[] decodeRunLengthMulti(ReadOnlySpan<byte> data)
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
					if (-offset > buffer.Count)
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
			var decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		private byte[] decodeRotate(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			byte code = 1;
			for (var i = 0; i < data.Length; i++)
			{
				buffer.Add(ror(data[i], code));
				code = (byte)(code + 2 & 7);
			}

			// convert to span
			var decodedSpan = new byte[buffer.Count];
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
			return (byte)(x >> shift | x << byteDigits - shift);
		}
	}
}

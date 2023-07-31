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
		public ILocoObject Load(string path)
		{
			if (!File.Exists(path))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {path}");
				return default;
			}

			Logger.Log(LogLevel.Info, $"Loading {path}");
			Span<byte> data = File.ReadAllBytes(path);

			var datFileHeader = DatFileHeader.Read(data[..Constants.DatFileHeaderSize]);
			var objHeader = ObjHeader.Read(data[Constants.DatFileHeaderSize..(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)]);
			var objSpan = Decode(objHeader.Encoding, data[(Constants.DatFileHeaderSize + Constants.ObjHeaderSize)..]);

			return datFileHeader.ObjectType switch
			{
				//ObjectType.bridge => MakeLocoObject<BridgeObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.building => MakeLocoObject<BuildingObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.cargo => MakeLocoObject<CargoObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.cliffEdge => MakeLocoObject<CliffEdgeObject>(datFileHeader, objHeader, objSpan),
				///ObjectType.climate => MakeLocoObject(datFileHeader, objHeader, ClimateObject.Read(objSpan)),
				///ObjectType.competitor => MakeLocoObject(datFileHeader, objHeader, CompetitorObject.Read(objSpan)),
				///ObjectType.currency => MakeLocoObject(datFileHeader, objHeader, CurrencyObject.Read(objSpan)),
				//ObjectType.dock => MakeLocoObject<DockObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.hillShapes => MakeLocoObject<HillShapesObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.industry => MakeLocoObject<IndustryObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.track => MakeLocoObject<TrackObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.trackSignal => MakeLocoObject<TrainSignalObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.tree => MakeLocoObject<TreeObject>(datFileHeader, objHeader, objSpan),
				//ObjectType.vehicle => MakeLocoObject<VehicleObject>(datFileHeader, objHeader, objSpan),
				_ => new LocoObject(datFileHeader, objHeader, new EmptyObject("<unknown>"))
			};
		}

		private static ILocoObject MakeLocoObject(DatFileHeader datFileHeader, ObjHeader objHeader, ILocoSubObject obj)
			=> new LocoObject(datFileHeader, objHeader, obj);

		//private static ILocoObject MakeLocoObject<T>(DatFileHeader datFileHeader, ObjHeader ObjHeader, ReadOnlySpan<byte> objSpan) where T : struct
		//	=> new LocoObject<T>(datFileHeader, ObjHeader, MemoryMarshal.Read<T>(objSpan));

		// taken from openloco's SawyerStreamReader::readChunk
		private ReadOnlySpan<byte> Decode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		{
			switch (encoding)
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

		// taken from openloco SawyerStreamReader::decodeRunLengthSingle
		private Span<byte> decodeRunLengthSingle(ReadOnlySpan<byte> data)
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

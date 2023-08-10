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
		public ILocoObject Load(FileInfo fileInfo)
		{
			var (ObjHdr1, ObjHDr2, Data) = LoadFromFile(fileInfo.FullName);
			var decodedData = Decode(ObjHDr2.Encoding, Data);
			var locoStruct = GetLocoStruct(ObjHdr1.ObjectType, decodedData);
			return new LocoObject(ObjHdr1, ObjHDr2, locoStruct, fileInfo.Name);
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

		public static ILocoStruct GetLocoStruct(ObjectType objectType, ReadOnlySpan<byte> data)
		{
			return objectType switch
			{
				ObjectType.airport => ByteReader.ReadLocoStruct<AirportObject>(data),
				ObjectType.bridge => ByteReader.ReadLocoStruct<BridgeObject>(data),
				ObjectType.building => ByteReader.ReadLocoStruct<BuildingObject>(data),
				ObjectType.cargo => ByteReader.ReadLocoStruct<CargoObject>(data),
				ObjectType.cliffEdge => ByteReader.ReadLocoStruct<CliffEdgeObject>(data),
				ObjectType.climate => ByteReader.ReadLocoStruct<ClimateObject>(data),
				ObjectType.competitor => ByteReader.ReadLocoStruct<CompetitorObject>(data),
				ObjectType.currency => ByteReader.ReadLocoStruct<CurrencyObject>(data),
				ObjectType.dock => ByteReader.ReadLocoStruct<DockObject>(data),
				ObjectType.hillShapes => ByteReader.ReadLocoStruct<HillShapesObject>(data),
				ObjectType.industry => ByteReader.ReadLocoStruct<IndustryObject>(data),
				ObjectType.interfaceSkin => ByteReader.ReadLocoStruct<InterfaceSkinObject>(data),
				ObjectType.land => ByteReader.ReadLocoStruct<LandObject>(data),
				ObjectType.levelCrossing => ByteReader.ReadLocoStruct<LevelCrossingObject>(data),
				ObjectType.region => ByteReader.ReadLocoStruct<RegionObject>(data),
				ObjectType.roadExtra => ByteReader.ReadLocoStruct<RoadExtraObject>(data),
				ObjectType.road => ByteReader.ReadLocoStruct<RoadObject>(data),
				ObjectType.roadStation => ByteReader.ReadLocoStruct<RoadStationObject>(data),
				ObjectType.scaffolding => ByteReader.ReadLocoStruct<ScaffoldingObject>(data),
				ObjectType.scenarioText => ByteReader.ReadLocoStruct<ScenarioTextObject>(data),
				ObjectType.snow => ByteReader.ReadLocoStruct<SnowObject>(data),
				//ObjectType.sound => ByteReader.ReadLocoStruct<SoundObject>(data),
				ObjectType.steam => ByteReader.ReadLocoStruct<SteamObject>(data),
				ObjectType.streetLight => ByteReader.ReadLocoStruct<StreetLightObject>(data),
				ObjectType.townNames => ByteReader.ReadLocoStruct<TownNamesObject>(data),
				ObjectType.trackExtra => ByteReader.ReadLocoStruct<TrackExtraObject>(data),
				ObjectType.track => ByteReader.ReadLocoStruct<TrackObject>(data),
				ObjectType.trainSignal => ByteReader.ReadLocoStruct<TrainSignalObject>(data),
				ObjectType.trainStation => ByteReader.ReadLocoStruct<TrainStationObject>(data),
				ObjectType.tree => ByteReader.ReadLocoStruct<TreeObject>(data),
				ObjectType.tunnel => ByteReader.ReadLocoStruct<TunnelObject>(data),
				//ObjectType.vehicle => MakeLocoObject<VehicleObject>(datFileHeader, objHeader, objSpan),
				ObjectType.wall => ByteReader.ReadLocoStruct<WallObject>(data),
				ObjectType.water => ByteReader.ReadLocoStruct<WaterObject>(data),
				_ => new EmptyObject($"{objectType} parsing not implemented yet"),
			};
		}

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

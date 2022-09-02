using System.Text;
using OpenLocoToolCommon;

namespace OpenLocoTool
{
	public static class Constants
	{
		public const int HeaderSize = 16; // bytes
		public const int LocoDatFileFlag = 0x11;
	}

	public enum SawyerEncoding : byte
	{
		uncompressed = 0,
		runLengthSingle = 1,
		runLengthMulti = 2,
		rotate = 3,
	}
	public enum S5Type : byte
	{
		savedGame = 0,
		scenario = 1,
		objects = 2,
		landscape = 3,
	};

	public class SawyerStreamReader
	{
		private readonly ILogger Logger;

		public SawyerStreamReader(ILogger logger)
			=> Logger = logger;

		// load file
		public ReadOnlySpan<byte> Load(string path)
		{
			if (!File.Exists(path))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {path}");
				return null;
			}

			Logger.Log(LogLevel.Info, $"Decoding {path}");
			return Load(File.ReadAllBytes(path));
		}

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> data)
		{
			var ptr = 0;

			// load header
			var headerSpan = data[..DatFileHeader.Size];
			var header = new DatFileHeader(headerSpan);
			ptr += DatFileHeader.Size;
			Logger.Log(LogLevel.Debug, header.ToString());

			var decodedObj = ReadChunk(data, ref ptr);

			// load rest of object

			return decodedObj;
		}

		// taken from openloco's SawyerStreamReader::readChunk
		private ReadOnlySpan<byte> ReadChunk(ReadOnlySpan<byte> data, ref int ptr)
		{
			// determine encoding
			var encoding = (SawyerEncoding)data[ptr];
			ptr += 1;
			Logger.Log(LogLevel.Debug, $"encoding={encoding}");

			// determine length
			var length = BitConverter.ToUInt32(data.Slice(ptr, 4));
			ptr += 4;
			Logger.Log(LogLevel.Debug, $"length={length}");

			var decoded = decode(encoding, data.Slice(ptr, (int)length));
			ptr += (int)length;

			return decoded;
		}

		private ReadOnlySpan<byte> decode(SawyerEncoding encoding, ReadOnlySpan<byte> dataSlice)
		{
			switch (encoding)
			{
				case SawyerEncoding.uncompressed:
					return dataSlice;
				case SawyerEncoding.runLengthSingle:
					return decodeRunLengthSingle(dataSlice);
				case SawyerEncoding.runLengthMulti:
					return decodeRunLengthMulti(decodeRunLengthSingle(dataSlice));
				case SawyerEncoding.rotate:
					return decodeRotate(dataSlice);
				default:
					Logger.Log(LogLevel.Error, "Unknown chunk encoding scheme");
					throw new InvalidDataException("Unknown encoding");
			}
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
					buffer.AddRange(Enumerable.Repeat(data[i + 1], copyLen));
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

	public enum ObjectType : byte
	{
		interfaceSkin,
		sound,
		currency,
		steam,
		rock,
		water,
		land,
		townNames,
		cargo,
		wall,
		trackSignal,
		levelCrossing,
		streetLight,
		tunnel,
		bridge,
		trackStation,
		trackExtra,
		track,
		roadStation,
		roadExtra,
		road,
		airport,
		dock,
		vehicle,
		tree,
		snow,
		climate,
		hillShapes,
		building,
		scaffolding,
		industry,
		region,
		competitor,
		scenarioText,
	};

	public class DatFileHeader
	{
		public const int Size = 0x10;

		public DatFileHeader(ReadOnlySpan<byte> data)
		{
			flags = BitConverter.ToUInt32(data.Slice(0, 4));

			name = data.Slice(4, 8).ToArray();

			checksum = BitConverter.ToUInt32(data.Slice(12, 4));
		}

		public override string ToString() => $"filename=\"{Encoding.ASCII.GetString(name)}\" sourceGame={getSourceGame()} type={getType()} isCustom={isCustom()} checksum={checksum}";

		public readonly uint flags;
		public readonly byte[] name = new byte[8] { 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf };
		public readonly uint checksum = 0xffffffff;

		private byte getSourceGame() => (byte)((flags >> 6) & 0x3);

		private ObjectType getType() => (ObjectType)(flags & 0x3F);

		private bool isCustom() => getSourceGame() == 0;

		//private bool isEmpty()
		//{
		//	auto ab = reinterpret_cast <const int64_t*> (this);
		//	return ab[0] == -1 && ab[1] == -1;
		//}
	}

	/*
	public class DatDecoder
	{
		private readonly ILogger Logger;

		public DatDecoder(ILogger logger)
			=> Logger = logger;

		public objclass Decode(string path)
		{
			if (!File.Exists(path))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {path}");
				return null;
			}

			Logger.Log(LogLevel.Info, $"Decoding {path}");
			return Decode(File.ReadAllBytes(path));
		}

		public objclass Decode(Span<byte> data)
		{
			var size = data.Length;
			Logger.Log(LogLevel.Info, $"File size is = {size} bytes");

			var ptr = 0;
			var header = new DatFileHeader(data.Slice(0, Constants.HeaderSize));
			ptr += Constants.HeaderSize;

			if (header.flags[3] != Constants.LocoDatFileFlag)
			{
				Logger.Log(LogLevel.Error, "Not a valid loco dat file");
				Logger.Log(LogLevel.Debug, header.ToString());
				return null;
			}
			else
			{
				Logger.Log(LogLevel.Debug, "This is a valid loco dat file");
			}

			// header is 2 bytes
			var objClass = header.@class;
			var objSubclass = header.subclass; //getvalue(data.Slice(1, 3));

			Logger.Log(LogLevel.Info, $"object class=0x{objClass:x} object subclass=0x{objSubclass:x} name={header.name}");

			var chunkCounter = 0;
			//objClass &= 0x7f;

			while (ptr < size)
			{
				Logger.Log(LogLevel.Debug2, $"Looping, ptr={ptr}");

				var encoding = (SawyerEncoding)data[ptr];
				ptr++;

				var chunkLength = getvalue(data.Slice(ptr, 4));
				ptr += 4;

				Logger.Log(LogLevel.Debug2, $"chunk[{chunkCounter}] encoding={encoding} chunkLength={chunkLength} ");

				chunkCounter++;
				var dataSlice = data.Slice(ptr, (int)chunkLength);
				// determine how to decode
				decode(encoding, dataSlice);

				ptr += (int)chunkLength;
			}

			Logger.Log(LogLevel.Info, "Finished decoding");
			return null;
		}

		private Span<byte> decode(SawyerEncoding encoding, Span<byte> dataSlice)
		{
			switch (encoding)
			{
				case SawyerEncoding.uncompressed:
					return dataSlice;
				case SawyerEncoding.runLengthSingle:
					return decodeRunLengthSingle(dataSlice);
				case SawyerEncoding.runLengthMulti:
					return decodeRunLengthMulti(decodeRunLengthSingle(dataSlice));
				case SawyerEncoding.rotate:
					return decodeRotate(dataSlice);
				default:
					Logger.Log(LogLevel.Error, "Unknown chunk encoding scheme");
					throw new InvalidDataException("Unknown encoding");
			}
		}

		//private Span<byte> readChunk(int maxDataLength)
		//{

		//}

		public static uint getvalue(ReadOnlySpan<byte> data)
		{
			uint result = 0;
			foreach (var b in data.ToArray().Reverse())
			{
				result <<= 8;
				result |= b;
			}

			return result;
		}

		private void dumpChunk(Span<byte> data, int objClass)
		{
			if (objClass >= ObjectClasses.ObjectClass.Count() || ObjectClasses.ObjectClass[objClass].desc == null)
			{
				Logger.Log(LogLevel.Error, $"Can't translate class {objClass:x} yet");
				return;
			}

			if (ObjectClasses.ObjectClass[objClass].desc[0].type != desctype.objdata)
			{
				Logger.Log(LogLevel.Error, "Object class has no objdata header");
				return;
			}

			// loop
			uint dumped = 0;

			for (var i = 0; ObjectClasses.ObjectClass[objClass].desc[i].type != desctype.END; i++)
			{
				var param = ObjectClasses.ObjectClass[objClass].desc[i].param;
				var type = ObjectClasses.ObjectClass[objClass].desc[i].type;

				switch (type)
				{
					case desctype.objdata:
						dumped += doDump(data.Slice((int)dumped), ObjectClasses.ObjectClass[objClass].vars, 1);
						break;
					default:
						break;
				}

			}
		}

		private const bool allunknown = false; // if 1, print all unknown variables, even empty ones

		private uint doDump(Span<byte> data, varinf[] vars, int indentation)
		{
			var name = string.Empty;
			var fname = string.Empty;
			var tag = string.Empty;
			uint ofs = 0;
			uint dumped = 0;
			uint totaldumped = 0;

			for (var i = 0; vars[i].ofs != -1; ++i)
			{
				var varinf = vars[i];

				if (varinf.name != null)
				{
					fname = varinf.name;
					Logger.Log(LogLevel.Debug, $"Field name={fname}");
				}

				if (ofs != varinf.ofs)
				{
					var errorMsg = $"Structure is invalid, ofs={ofs:x} but next field is {varinf.ofs:x}";
					Logger.Log(LogLevel.Warning, errorMsg);
					//throw new InvalidOperationException(errorMsg);
				}

				for (var j = 0; j < varinf.num; ++j)
				{
					//name = fname;
					//if (vars[i].num > 1)
					//{
					//	name = $"{name}{j}";
					//	Logger.Log(LogLevel.Debug2, name);
					//}

					var size = varinf.size;
					if (size <= 0)
					{
						if (vars[i].flags != null || vars[i].structvars == null)
						{
							//die("Variable %s has no size", name);
							throw new InvalidOperationException($"Variable {name} has no size");
						}

						if (vars[i].num > 1)
						{
							//die("Variable sized array members not supported");
							throw new InvalidOperationException("Variable sized array members not supported");
						}

						//nul = fopen("NUL", "wt");
						//if (!nul)
						//{
						//	//die("Can't write to NUL: %s", strerror(errno));
						//}

						//size = dodump(nul, data + ofs, vars[i].structvars, 0);
						//fclose(nul);

					}

					if (varinf.flags != null)
					{
						dumped = dumpflags(data.Slice((int)ofs, varinf.size), varinf.flags, indentation + 1);
					}
					else if (varinf.structvars != null)
					{
						dumped = doDump(data.Slice((int)ofs, varinf.size), varinf.structvars, indentation + 1);
					}
					else
					{
						var value = getvalue(data.Slice((int)ofs, varinf.size));

						size = Math.Abs(size);

						tag = "variable";
						if (varinf.name == null)
						{
							if (value > 0 || allunknown)
							{
								tag = "unknown";
							}
							else
							{
								tag = null; // don't print unset unknown variables
							}
						}

						if (tag != null)
						{
							Logger.Log(LogLevel.Debug2, $"tag={tag} name={name} size={size}");
						}
						//fprintf(xml, "%s<%s name=\"%s\" size=\"%X\">%d</%s>\n", INDENT, tag, name, size, value, tag);
						dumped = (uint)size;
					}
				}

				totaldumped += dumped;
			}

			return totaldumped;
		}

		// write a bit field
		private uint dumpflags(Span<byte> data, List<string> flags, int indentation)
		{
			uint value, i;

			//if (size > 4)
			//die("Can't dump flags with %d bytes", size);

			value = getvalue(data);

			for (i = 0; i < data.Length * 8; i++)
			{
				//if (*flags)
				//{
				//	name = *flags;
				//	flags++;
				//}
				//else
				//	name = string.Empty;

				//if (!strlen(name))
				//{
				//	sprintf(defname, "bit_%X", i);
				//	name = defname;
				//}
				//state = (value & (1 << i)) >> i;
				//if ((!onlysetbits && (name != defname)) || state)
				//	fprintf(xml, "%s<bit name=\"%s\">%d</bit>\n", INDENT, name, state);
			}

			return (uint)data.Length;
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthSingle
		private Span<byte> decodeRunLengthSingle(Span<byte> data)
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
					buffer.AddRange(Enumerable.Repeat(data[i + 1], copyLen));
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
		private Span<byte> decodeRunLengthMulti(Span<byte> data)
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

		private Span<byte> decodeRotate(Span<byte> data)
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

		// .net 7 (preview 5 and greater)
		//private T rotate<T>(T x, byte shift) where T : INumber<T>
		//{
		//	return (x >> shift) | (x << (limits::digits - shift));
		//}
	}
	*/
}

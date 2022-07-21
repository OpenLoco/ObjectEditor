using OpenLocoToolCommon;
using System.Drawing;
using System;
using System.Text;
using System.Xml.Linq;

namespace OpenLocoTool
{
	public static class Constants
	{
		public const int HeaderSize = 16; // bytes
		public const int LocoDatFileFlag = 0x11;
	}

	public class DatFileHeader
	{
		public DatFileHeader(ReadOnlySpan<byte> data)
		{
			flags[0] = data[0];
			flags[1] = data[1];
			flags[2] = data[2];
			flags[3] = data[3];

			subclass = DatDecoder.getvalue(data.Slice(1, 3));

			filename = Encoding.ASCII.GetString(data.Slice(4, 8));

			checksum = BitConverter.ToUInt32(data.Slice(12, 4));
		}

		public int @class => flags[0];

		public readonly byte[] flags = new byte[4];
		public readonly uint subclass;
		public readonly string filename;
		public readonly uint checksum;
	}

	public class DatDecoder
	{
		readonly ILogger Logger;

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
				return null;
			}
			else
			{
				Logger.Log(LogLevel.Debug, "This is a valid loco dat file");
			}

			// header is 2 bytes
			var objClass = header.@class;
			var objSubclass = header.subclass; //getvalue(data.Slice(1, 3));

			Logger.Log(LogLevel.Info, $"object class=0x{objClass:x} object subclass=0x{objSubclass:x} name={header.filename}");

			int chunkCounter = 0;
			objClass &= 0x7f;

			while (ptr < size)
			{
				Logger.Log(LogLevel.Debug2, $"Looping, ptr={ptr}");

				byte compression = data[ptr];
				ptr++;

				UInt32 chunkLength = getvalue(data.Slice(ptr, 4));
				ptr += 4;

				Logger.Log(LogLevel.Debug2, $"chunk[{chunkCounter}] compression=0x{compression} chunkLength={chunkLength} ");

				chunkCounter++;

				// determine how to decode
				switch (compression)
				{
					case 0: // none
						break;
					case 1: // rledecode
						var result = rleDecode(data.Slice(ptr, (int)chunkLength));
						dumpChunk(result, objClass);
						break;
					case 2: // decompress
						break;
					case 3: // descramble
						break;
					default:
						Logger.Log(LogLevel.Error, "Unknown chunk compression scheme");
						break;
				}

				ptr += (int)chunkLength;
			}

			Logger.Log(LogLevel.Info, "Finished decoding");
			return null;
		}

		public static UInt32 getvalue(ReadOnlySpan<byte> data)
		{
			UInt32 result = 0;
			foreach (byte b in data.ToArray().Reverse())
			{
				result <<= 8;
				result |= b;
			}

			return result;
		}

		void dumpChunk(Span<byte> data, int objClass)
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
			UInt32 dumped = 0;

			for (int i = 0; ObjectClasses.ObjectClass[objClass].desc[i].type != desctype.END; i++)
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

		const bool allunknown = false; // if 1, print all unknown variables, even empty ones

		UInt32 doDump(Span<byte> data, varinf[] vars, int indentation)
		{
			string name = string.Empty;
			string fname = string.Empty;
			string tag = string.Empty;
			UInt32 ofs = 0;
			UInt32 dumped = 0;
			UInt32 totaldumped = 0;

			for (int i = 0; vars[i].ofs != -1; ++i)
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

				for (int j = 0; j < varinf.num; ++j)
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
								tag = "unknown";
							else
								tag = null; // don't print unset unknown variables
						}

						if (tag != null)
							Logger.Log(LogLevel.Debug2, $"tag={tag} name={name} size={size}");
						//fprintf(xml, "%s<%s name=\"%s\" size=\"%X\">%d</%s>\n", INDENT, tag, name, size, value, tag);
						dumped = (UInt32)size;
					}
				}

				totaldumped += dumped;
			}

			return totaldumped;
		}

		// write a bit field
		UInt32 dumpflags(Span<byte> data, List<string> flags, int indentation)
		{
			UInt32 value, i, state;
			string name;

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

			return (UInt32)data.Length;
		}

		Span<byte> rleDecode(Span<byte> data)
		{
			List<(byte value, int length)> runs = new();
			List<byte> decoded = new();

			int i = 0;
			while (i < data.Length)
			{
				int rle = data[i++];
				int run = Math.Abs(rle) + 1;

				if (rle < 0)
				{
					byte b = data[i++];
					decoded.AddRange(Enumerable.Repeat(b, run));
					//runs.Add((b, run));
				}
				else
				{
					// uhoh
					if (i + run > data.Length)
					{
						break;
					}

					decoded.AddRange(data.Slice(i, run).ToArray());
					i += run;
					//runs.Add((, run));
				}


				//runs.Add((data[i], data[i + 1]));

				//count++;
			}

			Span<byte> decodedSpan = new byte[decoded.Count];
			int counter = 0;
			foreach (var b in decoded)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}
	}
}

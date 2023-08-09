namespace OpenLocoTool.DatFileParsing
{
	public static class StaticByteReader
	{
		public static uint8_t Read_uint8t(ReadOnlySpan<byte> data, int offset)
			=> data[offset];

		public static int8_t Read_int8t(ReadOnlySpan<byte> data, int offset)
			=> (sbyte)data[offset];

		public static uint16_t Read_uint16t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToUInt16(data[offset..(offset + 2)]);

		public static int16_t Read_int16t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToInt16(data[offset..(offset + 2)]);

		public static uint32_t Read_uint32t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToUInt32(data[offset..(offset + 4)]);

		public static int32_t Read_int32(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToInt32(data[offset..(offset + 4)]);

		// this method isn't necessary normally since you can just call the above methods ^
		// but it saves a lot of code rewriting for the Read_Array method
		public static T Read<T>(ReadOnlySpan<byte> data, int offset) where T : struct
		{
			var t = typeof(T);
			if (t == typeof(uint8_t))
			{
				return (dynamic)Read_uint8t(data, offset);
			}

			throw new NotImplementedException("");
		}

		public static T[] Read_Array<T>(ReadOnlySpan<byte> data, int offset, int count) where T : struct
		{
			var arr = new T[count];
			for (var i = 0; i < count; i++)
			{
				arr[i] = Read<T>(data, offset);
			}
			return arr;
		}
	}

	public ref struct ByteReader
	{
		public ByteReader(ReadOnlySpan<byte> data)
			=> this.data = data;

		ReadOnlySpan<byte> data { get; }
		int position;

		public uint8_t Read_uint8t()
		{
			var tmp = data[position];
			position += 1;
			return tmp;
		}

		public int8_t Read_int8t()
		{
			var tmp = (sbyte)data[position];
			position += 1;
			return tmp;
		}

		public uint16_t Read_uint16t()
		{
			var tmp = BitConverter.ToUInt16(data[position..(position + 2)]);
			position += 2;
			return tmp;
		}

		public int16_t Read_int16t()
		{
			var tmp = BitConverter.ToInt16(data[position..(position + 2)]);
			position += 2;
			return tmp;
		}

		public uint32_t Read_uint32t()
		{
			var tmp = BitConverter.ToUInt32(data[position..(position + 4)]);
			position += 4;
			return tmp;
		}

		public int32_t Read_int32t()
		{
			var tmp = BitConverter.ToInt32(data[position..(position + 4)]);
			position += 4;
			return tmp;
		}

		public int32_t Read_stringid()
		{
			return Read_uint16t();
		}

		// this method isn't necessary normally since you can just call the above methods ^
		// but it saves a lot of code rewriting for the Read_Array method
		public T Read<T>() where T : struct
			=> (T)ReadT(typeof(T));

		public T[] Read_Array<T>(int count) where T : struct
		{
			var arr = new T[count];
			for (var i = 0; i < count; i++)
			{
				arr[i] = Read<T>();
			}
			return arr;
		}

		public object ReadT(Type t, int arrLength = 0)
		{
			if (t == typeof(uint8_t))
			{
				return Read_uint8t();
			}
			if (t == typeof(int8_t))
			{
				return Read_int8t();
			}
			if (t == typeof(uint16_t))
			{
				return Read_uint16t();
			}
			if (t == typeof(int16_t))
			{
				return Read_int16t();
			}
			if (t == typeof(uint32_t))
			{
				return Read_uint32t();
			}
			if (t == typeof(int32_t))
			{
				return Read_int32t();
			}
			if (t == typeof(string_id))
			{
				return Read_stringid();
			}
			if (t.IsArray)
			{
				return Read_ArrayT(t, arrLength);
			}
			//if (t.IsEnum)
			//{
			//	var underlying = t.GetEnumUnderlyingType();
			//	var val = ReadT(underlying);

			//	// need special handling for loco flags :|
			//	//var enu = Enum.ToObject(t, val);
			//	//return enu;
			//	return Enum.GetValues(t).GetValue(0) ?? throw new ArgumentException($"{t}");
			//}

			if (t.IsEnum)
			{
				var underlyingType = t.GetEnumUnderlyingType();
				var underlyingValue = ReadT(underlyingType); // Read the underlying value

				if (t.IsDefined(typeof(FlagsAttribute), inherit: false))
				{
					var enumValues = Enum.GetValues(t);
					var combinedValue = 0;

					foreach (var enumValue in enumValues)
					{
						var enumValueInt = Convert.ToInt32(Enum.Parse(t, enumValue.ToString())); // Convert to int
						if ((enumValueInt & Convert.ToInt32(underlyingValue)) != 0) // Convert to int
						{
							combinedValue |= enumValueInt;
						}
					}

					var enu = Enum.ToObject(t, combinedValue);
					return enu;
				}
				else
				{
					var enu = Enum.ToObject(t, underlyingValue);
					return enu;
				}
			}

			throw new NotImplementedException(t.ToString());
		}

		public object Read_ArrayT(Type t, int length)
		{
			var elementType = t.GetElementType() ?? throw new ArgumentException($"Unable to get array element type for type {t}");
			var arr = Array.CreateInstance(elementType, length);
			for (var i = 0; i < length; i++)
			{
				arr.SetValue(ReadT(elementType), i);
			}
			return arr;
		}

		public static ILocoStruct ReadLocoStruct<T>(ReadOnlySpan<byte> data) where T : class
		{
			var properties = typeof(T).GetProperties();
			var args = new List<object>();
			var byteReader = new ByteReader(data);

			foreach (var p in properties)
			{
				// ignore non-binary properties on the records
				var shouldParse = p.GetCustomAttributes(typeof(LocoStructPropertyAttribute), inherit: false).Any();
				if (!shouldParse)
				{
					continue;
				}

				// special array handling
				if (p.PropertyType.IsArray)
				{
					var attrs = p.GetCustomAttributes(typeof(LocoArrayLengthAttribute), inherit: false);
					if (attrs.Length != 1)
					{
						throw new ArgumentOutOfRangeException(nameof(LocoArrayLengthAttribute), $"type {typeof(T)} with property {p} didn't have LocoArrayLength attribute specified");
					}
					args.Add(byteReader.ReadT(p.PropertyType, ((LocoArrayLengthAttribute)attrs[0]).Length));
				}
				else
				{
					args.Add(byteReader.ReadT(p.PropertyType));
				}
			}

			try
			{
				var argsArr = args.ToArray();
				if (argsArr.Length == 0)
				{
					throw new ArgumentException($"{nameof(argsArr)} had no arguments to construct an object");
				}

				var newInstance = Activator.CreateInstance(typeof(T), argsArr);
				return newInstance is T instance
					? (ILocoStruct)instance
					: throw new InvalidDataException("couldn't parse");
			}
			catch (Exception ex)
			{
				return default;
			}
		}
	}
}

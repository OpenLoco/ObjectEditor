namespace OpenLocoTool.DatFileParsing
{
	public static class ByteReader
	{
		public static int GetObjectSize(Type type)
		{
			var size = 0;
			if (type == typeof(byte) || type == typeof(sbyte))
			{
				size = 1;
			}
			else if (type == typeof(uint16_t) || type == typeof(int16_t))
			{
				size = 2;
			}
			else if (type == typeof(uint32_t) || type == typeof(int32_t))
			{
				size = 4;
			}
			else
			{
				var sizeAttr = AttributeHelper.Get<LocoStructSizeAttribute>(type) ?? throw new ArgumentOutOfRangeException(nameof(LocoStructSizeAttribute), $"type {type} didn't have LocoStructSizeAttribute");
				size = sizeAttr.Size;
			}

			if (size == 0)
			{
				throw new ArgumentException("unknown primitive type with no size");
			}

			return size;
		}

		public static object ReadT(ReadOnlySpan<byte> data, Type t, int position, int arrLength = 0)
		{
			if (t == typeof(uint8_t))
			{
				return ByteReaderT.Read_uint8t(data, position);
			}

			if (t == typeof(int8_t))
			{
				return ByteReaderT.Read_int8t(data, position);
			}

			if (t == typeof(uint16_t))
			{
				return ByteReaderT.Read_uint16t(data, position);
			}

			if (t == typeof(int16_t))
			{
				return ByteReaderT.Read_int16t(data, position);
			}

			if (t == typeof(uint32_t))
			{
				return ByteReaderT.Read_uint32t(data, position);
			}

			if (t == typeof(int32_t))
			{
				return ByteReaderT.Read_int32t(data, position);
			}

			if (t == typeof(string_id))
			{
				return ByteReaderT.Read_uint16t(data, position);
			}

			if (t.IsArray)
			{
				var elementType = t.GetElementType();
				var size = GetObjectSize(elementType);

				var arr = Array.CreateInstance(elementType, arrLength);
				for (var i = 0; i < arrLength; i++)
				{
					arr.SetValue(ReadT(data, elementType, position + (i * size)), i);
				}

				return arr;
			}

			if (t.IsEnum) // this is so big because we need special handling for 'flags' enums
			{
				var underlyingType = t.GetEnumUnderlyingType();
				var underlyingValue = ReadT(data, underlyingType, position); // Read the underlying value

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

			if (t.IsClass)
			{
				var objectSize = GetObjectSize(t);
				return ReadLocoStruct(data[position..(position + objectSize)], t);
			}

			throw new NotImplementedException(t.ToString());
		}

		public static ILocoStruct ReadLocoStruct<T>(ReadOnlySpan<byte> data) where T : class
			=> ReadLocoStruct(data, typeof(T));

		public static ILocoStruct ReadLocoStruct(ReadOnlySpan<byte> data, Type t)
		{
			var properties = t.GetProperties();
			var args = new List<object>();

			foreach (var p in properties)
			{
				// ignore non-loco properties on the records
				var offsetAttr = AttributeHelper.Get<LocoStructOffsetAttribute>(p);
				if (offsetAttr == null)
				{
					continue;
				}

				// special array handling
				var arrLength = 0;
				if (p.PropertyType.IsArray)
				{
					var arrLengthAttr = AttributeHelper.Get<LocoArrayLengthAttribute>(p) ?? throw new ArgumentOutOfRangeException(nameof(LocoArrayLengthAttribute), $"type {t} with property {p} didn't have LocoArrayLength attribute specified");
					arrLength = arrLengthAttr.Length;
				}

				args.Add(ReadT(data, p.PropertyType, offsetAttr.Offset, arrLength));
			}

			return (ILocoStruct?)Activator.CreateInstance(t, args.ToArray()) ?? throw new InvalidDataException("couldn't parse");
		}
	}
}

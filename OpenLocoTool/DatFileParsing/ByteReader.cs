using OpenLocoTool.Objects;

namespace OpenLocoTool.DatFileParsing
{
	public static class ByteReader
	{
		public static object ReadT(ReadOnlySpan<byte> data, Type t, int offset, int arrLength = 0)
		{
			if (t == typeof(uint8_t))
			{
				return ByteReaderT.Read_uint8t(data, offset);
			}

			if (t == typeof(int8_t))
			{
				return ByteReaderT.Read_int8t(data, offset);
			}

			if (t == typeof(uint16_t))
			{
				return ByteReaderT.Read_uint16t(data, offset);
			}

			if (t == typeof(int16_t))
			{
				return ByteReaderT.Read_int16t(data, offset);
			}

			if (t == typeof(uint32_t))
			{
				return ByteReaderT.Read_uint32t(data, offset);
			}

			if (t == typeof(int32_t))
			{
				return ByteReaderT.Read_int32t(data, offset);
			}

			if (t == typeof(string_id))
			{
				return ByteReaderT.Read_uint16t(data, offset);
			}

			if (t.IsArray)
			{
				var elementType = t.GetElementType();
				var size = ByteHelpers.GetObjectSize(elementType);

				var arr = Array.CreateInstance(elementType, arrLength);
				for (var i = 0; i < arrLength; i++)
				{
					arr.SetValue(ReadT(data, elementType, offset + (i * size)), i); // why pass 'i' in here?
				}

				return arr;
			}

			if (t.IsEnum) // this is so big because we need special handling for 'flags' enums
			{
				var underlyingType = t.GetEnumUnderlyingType();
				var underlyingValue = ReadT(data, underlyingType, offset);

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
				var objectSize = ByteHelpers.GetObjectSize(t);
				return ReadLocoStruct(data[offset..(offset + objectSize)], t);
			}

			throw new NotImplementedException(t.ToString());
		}

		public static T ReadLocoStruct<T>(ReadOnlySpan<byte> data) where T : class
			=> (T)ReadLocoStruct(data, typeof(T));

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

				// ignore skipped properties (usually image ids and string ids which are only used in loco itself, not this tool
				var skip = AttributeHelper.Get<LocoStructSkipReadAttribute>(p);
				if (skip != null)
				{
					continue;
				}

				// special array handling
				var arrLength = 0;
				if (p.PropertyType.IsArray)
				{
					var arrLengthAttr = AttributeHelper.Get<LocoArrayLengthAttribute>(p) ?? throw new ArgumentOutOfRangeException(nameof(data), $"type {t} with property {p} didn't have LocoArrayLength attribute specified");
					arrLength = arrLengthAttr.Length;
				}

				// ignore pointers/variable data - they'll be loaded later in Load()
				var variableAttr = AttributeHelper.Get<LocoStructVariableLoadAttribute>(p);
				if (variableAttr != null)
				{
					if (p.PropertyType.IsArray && p.PropertyType.GetElementType() == typeof(uint8_t))
					{
						args.Add(new uint8_t[arrLength]);
					}
					else
					{
						args.Add(Activator.CreateInstance(p.PropertyType));
					}

					continue;
				}

				args.Add(ReadT(data, p.PropertyType, offsetAttr.Offset, arrLength));
			}

			return (ILocoStruct?)Activator.CreateInstance(t, [.. args]) ?? throw new InvalidDataException("couldn't parse");
		}

		public static IList<ILocoStruct> ReadLocoStructArray(ReadOnlySpan<byte> data, Type t, int count, int structSize) // could get struct size from attribute, but easier just to pass in
		{
			// cannot use ReadOnlySpan with yield return :|
			var list = new List<ILocoStruct>();
			for (var i = 0; i < count; ++i)
			{
				var range = data[(i * structSize)..((i + 1) * structSize)];
				list.Add(ReadLocoStruct(range, t));
			}

			return list;
		}
	}
}

using Dat.Types;
using Definitions.ObjectModels;

namespace Dat.FileParsing;

public static class ByteReader
{
	public static object ReadT(ReadOnlySpan<byte> data, Type t, int offset, int arrLength = 0, bool isVariableLoad = false)
	{
		if (t == typeof(uint8_t))
		{
			var val = ByteReaderT.Read_uint8t(data, offset);
			if (isVariableLoad && val != 0)
			{
				throw new InvalidDataException($"uint8_t at offset {offset} had non-zero variable-load data. Value={val}");
			}

			return val;
		}

		if (t == typeof(int8_t))
		{
			var val = ByteReaderT.Read_int8t(data, offset);
			if (isVariableLoad && val != 0)
			{
				throw new InvalidDataException($"int8_t at offset {offset} had non-zero variable-load data. Value={val}");
			}

			return val;
		}

		if (t == typeof(uint16_t))
		{
			var val = ByteReaderT.Read_uint16t(data, offset);
			if (isVariableLoad && val != 0)
			{
				throw new InvalidDataException($"uint16_t at offset {offset} had non-zero variable-load data. Value={val}");
			}

			return val;
		}

		if (t == typeof(int16_t))
		{
			var val = ByteReaderT.Read_int16t(data, offset);
			if (isVariableLoad && val != 0)
			{
				throw new InvalidDataException($"int16_t at offset {offset} had non-zero variable-load data. Value={val}");
			}

			return val;
		}

		if (t == typeof(uint32_t))
		{
			var val = ByteReaderT.Read_uint32t(data, offset);
			if (isVariableLoad && val != 0)
			{
				throw new InvalidDataException($"uint32_t at offset {offset} had non-zero variable-load data. Value={val}");
			}

			return val;
		}

		if (t == typeof(int32_t))
		{
			var val = ByteReaderT.Read_int32t(data, offset);
			if (isVariableLoad && val != 0)
			{
				throw new InvalidDataException($"int32_t at offset {offset} had non-zero variable-load data. Value={val}");
			}

			return val;
		}

		if (t == typeof(string_id))
		{
			return ByteReaderT.Read_uint16t(data, offset);
		}

		if (t == typeof(bool))
		{
			return ByteReaderT.Read_bool(data, offset);
		}

		if (t.IsArray)
		{
			return ReadArray(data, t, offset, arrLength, isVariableLoad);
		}

		if (t.IsEnum) // this is so big because we need special handling for 'flags' enums
		{
			return ReadEnum(data, t, offset);
		}

		if (t.IsClass)
		{
			return ReadClass(data, t, offset);
		}

		throw new NotImplementedException(t.ToString());
	}

	static Array ReadArray(ReadOnlySpan<byte> data, Type t, int offset, int arrLength, bool isVariableLoad)
	{
		var elementType = t.GetElementType() ?? throw new ArgumentNullException(t.Name);
		var size = ByteHelpers.GetObjectSize(elementType);

		var arr = Array.CreateInstance(elementType, arrLength);
		for (var i = 0; i < arrLength; i++)
		{
			arr.SetValue(ReadT(data, elementType, offset + (i * size), isVariableLoad: isVariableLoad), i); // why pass 'i' in here?
		}

		return arr;
	}

	static object ReadClass(ReadOnlySpan<byte> data, Type t, int offset)
	{
		if (t.Name == "ObjectHeader")
		{
			return ObjectHeader.Read(data[..ObjectHeader.StructLength]);
		}
		else if (t.Name == "S5Header")
		{
			return S5Header.Read(data[..S5Header.StructLength]);
		}

		var objectSize = ByteHelpers.GetObjectSize(t);
		return ReadLocoStruct(data[offset..(offset + objectSize)], t);
	}

	static object ReadEnum(ReadOnlySpan<byte> data, Type t, int offset)
	{
		var underlyingType = t.GetEnumUnderlyingType();
		var underlyingValue = ReadT(data, underlyingType, offset);

		if (!t.IsDefined(typeof(FlagsAttribute), inherit: false))
		{
			return Enum.ToObject(t, underlyingValue);
		}

		var enumValues = Enum.GetValues(t);

		if (underlyingType == typeof(int8_t) || underlyingType == typeof(int16_t) || underlyingType == typeof(int32_t))
		{
			var combinedValue = 0;
			foreach (var enumValue in enumValues)
			{
				var parsed = Enum.Parse(t, enumValue.ToString()!);
				var enumValueInt = Convert.ToInt32(parsed); // Convert to int
				if ((enumValueInt & Convert.ToInt32(underlyingValue)) != 0) // Convert to int
				{
					combinedValue |= enumValueInt;
				}
			}

			return Enum.ToObject(t, combinedValue);
		}
		else if (underlyingType == typeof(uint8_t) || underlyingType == typeof(uint16_t) || underlyingType == typeof(uint32_t))
		{
			var combinedValue = 0U;
			foreach (var enumValue in enumValues)
			{
				var parsed = Enum.Parse(t, enumValue.ToString()!);
				var enumValueInt = Convert.ToUInt32(parsed); // Convert to int
				if ((enumValueInt & Convert.ToUInt32(underlyingValue)) != 0) // Convert to int
				{
					combinedValue |= enumValueInt;
				}
			}

			return Enum.ToObject(t, combinedValue);
		}
		else
		{
			throw new ArgumentOutOfRangeException(nameof(underlyingType), underlyingType, "unrecognised type");
		}
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
				if (p.PropertyType.IsArray)
				{
					// todo: find a generic way to do this
					if (p.PropertyType.GetElementType() == typeof(uint8_t))
					{
						args.Add(new uint8_t[arrLength]);
					}
					else if (p.PropertyType.GetElementType() == typeof(int8_t))
					{
						args.Add(new int8_t[arrLength]);
					}
					else if (p.PropertyType.GetElementType() == typeof(uint16_t))
					{
						args.Add(new uint16_t[arrLength]);
					}
					else if (p.PropertyType.GetElementType() == typeof(int16_t))
					{
						args.Add(new int16_t[arrLength]);
					}
					else if (p.PropertyType.GetElementType() == typeof(uint32_t))
					{
						args.Add(new uint32_t[arrLength]);
					}
					else if (p.PropertyType.GetElementType() == typeof(int32_t))
					{
						args.Add(new int32_t[arrLength]);
					}
				}
				else
				{
					var newInstance = Activator.CreateInstance(p.PropertyType);
					ArgumentNullException.ThrowIfNull(newInstance, paramName: p.PropertyType.Name);
					args.Add(newInstance!);
				}

				continue;
			}

			args.Add(ReadT(data, p.PropertyType, offsetAttr.Offset, arrLength, variableAttr != null));
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

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
			arr.SetValue(ReadT(data, elementType, offset + (i * size), isVariableLoad: isVariableLoad), i);
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

		// Note: a plain (non-flags) enum simply round-trips the underlying value.
		var enumMetadata = LocoEnumMetadataCache.Get(t);
		if (!enumMetadata.IsFlags)
		{
			return Enum.ToObject(t, underlyingValue);
		}

		// Flag members are cached once per type; OR together every member that intersects the
		// raw value (this drops bits that don't map to a declared flag).
		if (enumMetadata.IsSigned)
		{
			var value = Convert.ToInt32(underlyingValue);
			var combinedValue = 0;
			foreach (var enumValueInt in enumMetadata.SignedValues)
			{
				if ((enumValueInt & value) != 0)
				{
					combinedValue |= enumValueInt;
				}
			}

			return Enum.ToObject(t, combinedValue);
		}
		else
		{
			var value = Convert.ToUInt32(underlyingValue);
			var combinedValue = 0U;
			foreach (var enumValueInt in enumMetadata.UnsignedValues)
			{
				if ((enumValueInt & value) != 0)
				{
					combinedValue |= enumValueInt;
				}
			}

			return Enum.ToObject(t, combinedValue);
		}
	}

	public static T ReadLocoStruct<T>(ReadOnlySpan<byte> data) where T : class
		=> (T)ReadLocoStruct(data, typeof(T));

	public static ILocoStruct ReadLocoStruct(ReadOnlySpan<byte> data, Type t)
	{
		var properties = LocoStructMetadata.GetStructProperties(t);
		var args = new List<object>(properties.Length);

		foreach (var p in properties)
		{
			// ignore skipped properties (usually image ids and string ids which are only used in loco itself, not this tool)
			if (p.IsSkipRead)
			{
				continue;
			}

			// ignore pointers/variable data - they'll be loaded later in Load()
			if (p.IsVariableLoad)
			{
				if (p.IsArray)
				{
					// todo: find a generic way to do this
					var defaultArray = CreateDefaultArray(p.ElementType!, p.ArrayLength);
					if (defaultArray != null)
					{
						args.Add(defaultArray);
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

			args.Add(ReadT(data, p.PropertyType, p.Offset, p.ArrayLength));
		}

		return (ILocoStruct?)Activator.CreateInstance(t, [.. args]) ?? throw new InvalidDataException("couldn't parse");
	}

	// Creates a zero-filled typed array for a variable-load array property, matching the loco
	// integer aliases. Returns null for unrecognised element types (nothing was added historically).
	static object? CreateDefaultArray(Type elementType, int length)
	{
		if (elementType == typeof(uint8_t))
		{
			return new uint8_t[length];
		}

		if (elementType == typeof(int8_t))
		{
			return new int8_t[length];
		}

		if (elementType == typeof(uint16_t))
		{
			return new uint16_t[length];
		}

		if (elementType == typeof(int16_t))
		{
			return new int16_t[length];
		}

		if (elementType == typeof(uint32_t))
		{
			return new uint32_t[length];
		}

		if (elementType == typeof(int32_t))
		{
			return new int32_t[length];
		}

		return null;
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

	//public static IList<ILocoStruct> ReadLocoStructArray(ReadOnlySpan<byte> data, Type t, int count)
	//{
	//	// cannot use ReadOnlySpan with yield return :|
	//	var structSize = data.Length / count;
	//	var list = new List<ILocoStruct>();
	//	for (var i = 0; i < count; ++i)
	//	{
	//		var range = data[(i * structSize)..((i + 1) * structSize)];
	//		list.Add(ReadLocoStruct(range, t));
	//	}

	//	return list;
	//}

	public static IList<T> ReadLocoStructArray<T>(ReadOnlySpan<byte> data, int count, int structSize) where T : class
	{
		// cannot use ReadOnlySpan with yield return :|
		var list = new List<T>();
		for (var i = 0; i < count; ++i)
		{
			var range = data[(i * structSize)..((i + 1) * structSize)];
			list.Add(ReadLocoStruct<T>(range));
		}

		return list;
	}
}

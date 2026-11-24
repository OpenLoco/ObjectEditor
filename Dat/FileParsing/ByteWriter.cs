using Definitions.ObjectModels;

namespace Dat.FileParsing;

public static class ByteWriter
{
	public static void WriteT(Span<byte> data, Type t, int offset, object val)
	{
		// The values reaching here are boxed primitives of the property/element type, or a boxed
		// int 0 for skip-read properties. Convert.* performs the same coercion the previous
		// (uint8_t)(dynamic)val casts did, but without the DLR call-site overhead.
		if (t == typeof(uint8_t))
		{
			ByteWriterT.Write_uint8t(data, offset, Convert.ToByte(val));
		}
		else if (t == typeof(int8_t))
		{
			ByteWriterT.Write_int8t(data, offset, Convert.ToSByte(val));
		}
		else if (t == typeof(uint16_t))
		{
			ByteWriterT.Write_uint16t(data, offset, Convert.ToUInt16(val));
		}
		else if (t == typeof(int16_t))
		{
			ByteWriterT.Write_int16t(data, offset, Convert.ToInt16(val));
		}
		else if (t == typeof(uint32_t))
		{
			ByteWriterT.Write_uint32t(data, offset, Convert.ToUInt32(val));
		}
		else if (t == typeof(int32_t))
		{
			ByteWriterT.Write_int32t(data, offset, Convert.ToInt32(val));
		}
		else if (t == typeof(string_id))
		{
			// string ids should always be 0 in the dat file - they're only set when loaded into memory and never saved
			ByteWriterT.Write_uint16t(data, offset, 0);
		}
		else if (t == typeof(bool))
		{
			ByteWriterT.Write_bool(data, offset, Convert.ToBoolean(val));
		}
		else if (t.IsArray)
		{
			var elementType = t.GetElementType() ?? throw new ArgumentNullException(t.Name);
			var size = ByteHelpers.GetObjectSize(elementType);
			var arr = (Array)val;

			for (var i = 0; i < arr.Length; i++)
			{
				var value = arr.GetValue(i) ?? throw new ArgumentNullException($"{t.Name}[{i}]");
				WriteT(data, elementType, offset + (i * size), value);
			}
		}
		else if (t.IsEnum)
		{
			var underlyingType = t.GetEnumUnderlyingType();
			var underlyingValue = Convert.ChangeType(val, underlyingType);
			WriteT(data, underlyingType, offset, underlyingValue);
		}
		else if (t.IsClass)
		{
			var objectSize = ByteHelpers.GetObjectSize(t);
			var bytes = WriteLocoStruct((ILocoStruct)val);

			if (bytes.Length != objectSize)
			{
				throw new InvalidOperationException();
			}

			bytes.CopyTo(data[offset..(offset + objectSize)]);
		}
		else
		{
			throw new InvalidOperationException("how");
		}
	}

	public static ReadOnlySpan<byte> WriteLocoStruct(ILocoStruct obj)
	{
		ArgumentNullException.ThrowIfNull(obj);

		var t = obj.GetType();
		var objSize = ByteHelpers.GetObjectSize(t);
		var buf = new byte[objSize];

		foreach (var p in LocoStructMetadata.GetStructProperties(t))
		{
			// write 0s to properties that need it
			if (p.IsSkipRead)
			{
				WriteT(buf, p.PropertyType, p.Offset, 0);
				continue;
			}

			// skip variable struct loading as they'll be set in special save
			if (p.IsVariableLoad)
			{
				continue;
			}

			var propVal = p.Property.GetValue(obj) ?? throw new ArgumentNullException($"{p.Property.Name} for {obj} was null");
			WriteT(buf, p.PropertyType, p.Offset, propVal);
		}

		return buf;
	}
}

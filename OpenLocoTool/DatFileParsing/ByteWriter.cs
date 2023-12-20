namespace OpenLocoTool.DatFileParsing
{
	public static class ByteWriter
	{
		public static void WriteT(Span<byte> data, Type t, int offset, object val)
		{
			if (t == typeof(uint8_t))
			{
				ByteWriterT.Write(data, offset, (uint8_t)(dynamic)val);
			}

			else if (t == typeof(int8_t))
			{
				ByteWriterT.Write(data, offset, (int8_t)(dynamic)val);
			}

			else if (t == typeof(uint16_t))
			{
				ByteWriterT.Write(data, offset, (uint16_t)(dynamic)val);
			}

			else if (t == typeof(int16_t))
			{
				ByteWriterT.Write(data, offset, (int16_t)(dynamic)val);
			}

			else if (t == typeof(uint32_t))
			{
				ByteWriterT.Write(data, offset, (uint32_t)(dynamic)val);
			}

			else if (t == typeof(int32_t))
			{
				ByteWriterT.Write(data, offset, (int32_t)(dynamic)val);
			}

			else if (t == typeof(string_id))
			{
				// string ids should always be 0 in the dat file - they're only set when loaded into memory and never saved
				val = 0;
				ByteWriterT.Write(data, offset, (string_id)(dynamic)val);
			}

			else if (t.IsArray)
			{
				var elementType = t.GetElementType() ?? throw new NullReferenceException();
				var size = ByteHelpers.GetObjectSize(elementType);
				var arr = (Array)val;

				for (var i = 0; i < arr.Length; i++)
				{
					var value = arr.GetValue(i) ?? throw new NullReferenceException();
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
			if (obj == null)
			{
				throw new NullReferenceException();
			}

			var t = obj.GetType();
			var objSize = ByteHelpers.GetObjectSize(t);
			var buf = new byte[objSize];
			var span = buf.AsSpan();

			foreach (var p in t.GetProperties())
			{
				// ignore non-loco properties on the records
				var offsetAttr = AttributeHelper.Get<LocoStructOffsetAttribute>(p);
				if (offsetAttr == null)
				{
					continue;
				}

				// write 0s to properties that need it
				var skip = AttributeHelper.Get<LocoStructSkipReadAttribute>(p);
				if (skip != null)
				{
					WriteT(buf, p.PropertyType, offsetAttr.Offset, 0);
					continue;
				}

				// special array handling
				var arrLength = 0;
				if (p.PropertyType.IsArray)
				{
					var arrLengthAttr = AttributeHelper.Get<LocoArrayLengthAttribute>(p) ?? throw new ArgumentOutOfRangeException(nameof(LocoArrayLengthAttribute), $"type {t} with property {p} didn't have LocoArrayLength attribute specified");
					arrLength = arrLengthAttr.Length;
				}

				var propVal = p.GetValue(obj) ?? throw new NullReferenceException();
				WriteT(buf, p.PropertyType, offsetAttr.Offset, propVal);
			}

			return buf;
		}
	}
}

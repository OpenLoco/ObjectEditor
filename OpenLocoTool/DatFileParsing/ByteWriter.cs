using OpenLocoTool.Objects;

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

			if (t == typeof(int8_t))
			{
				ByteWriterT.Write(data, offset, (int8_t)(dynamic)val);
			}

			if (t == typeof(uint16_t))
			{
				ByteWriterT.Write(data, offset, (uint16_t)(dynamic)val);
			}

			if (t == typeof(int16_t))
			{
				ByteWriterT.Write(data, offset, (int16_t)(dynamic)val);
			}

			if (t == typeof(uint32_t))
			{
				ByteWriterT.Write(data, offset, (uint32_t)(dynamic)val);
			}

			if (t == typeof(int32_t))
			{
				ByteWriterT.Write(data, offset, (int32_t)(dynamic)val);
			}

			if (t == typeof(string_id))
			{
				ByteWriterT.Write(data, offset, (string_id)(dynamic)val);
			}

			if (t.IsArray)
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

			if (t.IsEnum)
			{
				throw new NotImplementedException(t.ToString());
			}

			if (t.IsClass)
			{
				var objectSize = ByteHelpers.GetObjectSize(t);
				//return WriteLocoStruct(data[offset..(offset + objectSize)], t);
				throw new NotImplementedException(t.ToString());
			}
		}

		public static ReadOnlySpan<byte> WriteLocoObject(ILocoObject obj)
		{
			var objBytes = Bytes(obj.Object);
			var ms = new MemoryStream();
			ms.Write(objBytes);

			var stringBytes = Bytes(obj.StringTable);
			ms.Write(stringBytes);

			if (obj.Object is ILocoStructVariableData objV)
			{
				var variableBytes = objV.Save();
				ms.Write(variableBytes);
			}

			if (obj.G1Header.NumEntries != 0 && obj.G1Elements.Count != 0)
			{
				var g1Bytes = Bytes(obj.G1Header);
				ms.Write(g1Bytes);

				var g1ElementsBytes = Bytes(obj.G1Elements);
				ms.Write(g1ElementsBytes);
			}

			ms.Flush();
			ms.Close();

			return ms.ToArray();
		}

		public static ReadOnlySpan<byte> Bytes(object obj)
		{
			// todo: this just a skeleton placeholder method
			// we need to implement Save() for image table and G1 data
			throw new NotImplementedException();
			return new byte[1];
		}
	}
}

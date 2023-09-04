namespace OpenLocoTool.DatFileParsing
{
	public static class ByteReaderT
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

		public static int32_t Read_int32t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToInt32(data[offset..(offset + 4)]);

		public static T Read<T>(ReadOnlySpan<byte> data, int offset) where T : struct
		{
			if (typeof(T) == typeof(uint8_t))
				return (T)(dynamic)Read_uint8t(data, offset);
			if (typeof(T) == typeof(int8_t))
				return (T)(dynamic)Read_int8t(data, offset);

			if (typeof(T) == typeof(uint16_t))
				return (T)(dynamic)Read_uint16t(data, offset);
			if (typeof(T) == typeof(int16_t))
				return (T)(dynamic)Read_int16t(data, offset);

			if (typeof(T) == typeof(uint32_t))
				return (T)(dynamic)Read_uint32t(data, offset);
			if (typeof(T) == typeof(int32_t))
				return (T)(dynamic)Read_int32t(data, offset);

			throw new NotImplementedException("");
		}

		public static T[] Read_Array<T>(ReadOnlySpan<byte> data, int count, int offset = 0) where T : struct
		{
			var arr = new T[count];
			var typeSize = ByteReader.GetObjectSize(typeof(T));
			for (var i = 0; i < count; i++)
			{
				arr[i] = Read<T>(data, offset + (i * typeSize));
			}

			return arr;
		}
	}
}

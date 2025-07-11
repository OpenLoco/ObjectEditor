namespace Dat.FileParsing
{
	public static class ByteWriterT
	{
		public static void Write_uint8t(Span<byte> data, int offset, uint8_t val)
			=> data[offset] = val;

		public static void Write_int8t(Span<byte> data, int offset, int8_t val)
			=> data[offset] = (byte)val;

		public static void Write_bool(Span<byte> data, int offset, bool val)
			=> BitConverter.TryWriteBytes(data[offset..(offset + 1)], val);

		public static void Write_uint16t(Span<byte> data, int offset, uint16_t val)
			=> BitConverter.TryWriteBytes(data[offset..(offset + 2)], val);

		public static void Write_int16t(Span<byte> data, int offset, int16_t val)
			=> BitConverter.TryWriteBytes(data[offset..(offset + 2)], val);

		public static void Write_uint32t(Span<byte> data, int offset, uint32_t val)
			=> BitConverter.TryWriteBytes(data[offset..(offset + 4)], val);

		public static void Write_int32t(Span<byte> data, int offset, int32_t val)
			=> BitConverter.TryWriteBytes(data[offset..(offset + 4)], val);

		public static void Write<T>(Span<byte> data, int offset, T val) where T : struct
		{
			if (typeof(T) == typeof(uint8_t))
			{
				Write_uint8t(data, offset, (uint8_t)(dynamic)val);
			}
			else if (typeof(T) == typeof(int8_t))
			{
				Write_int8t(data, offset, (int8_t)(dynamic)val);
			}
			else if (typeof(T) == typeof(uint16_t))
			{
				Write_uint16t(data, offset, (uint16_t)(dynamic)val);
			}
			else if (typeof(T) == typeof(int16_t))
			{
				Write_int16t(data, offset, (int16_t)(dynamic)val);
			}
			else if (typeof(T) == typeof(uint32_t))
			{
				Write_uint32t(data, offset, (uint32_t)(dynamic)val);
			}
			else if (typeof(T) == typeof(int32_t))
			{
				Write_int32t(data, offset, (int32_t)(dynamic)val);
			}
			else if (typeof(T) == typeof(bool))
			{
				Write_bool(data, offset, (bool)(dynamic)val);
			}
			else
			{
				throw new NotImplementedException($"{typeof(T)}");
			}
		}

		public static T[] Write_Array<T>(Span<byte> data, int count, int offset = 0) where T : struct
		{
			var arr = new T[count];
			var typeSize = ByteHelpers.GetObjectSize(typeof(T));
			for (var i = 0; i < count; i++)
			{
				Write<T>(data, offset + (i * typeSize), arr[i]);
			}

			return arr;
		}
	}
}

namespace OpenLoco.Dat.FileParsing
{
	public static class ByteHelpers
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

			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size, nameof(size));

			return size;
		}
	}
}

namespace OpenLocoTool.DatFileParsing
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoArrayLengthAttribute : Attribute
	{
		public LocoArrayLengthAttribute(int length) => Length = length;
		public int Length { get; }
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructOffsetAttribute : Attribute
	{
		public LocoStructOffsetAttribute(int offset) => Offset = offset;

		public int Offset { get; }
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class LocoStructSizeAttribute : Attribute
	{
		public LocoStructSizeAttribute(int size) => Size = size;

		public int Size { get; }
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class LocoStringTableAttribute : Attribute
	{
		public LocoStringTableAttribute(params string[] names)
		{
			Names = names;
		}

		public string[] Names { get; }

		public int Count => Names.Length;
	}

	// basically a 'skip' attribute to allow deferred loading for variable data
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructVariableLoadAttribute : Attribute
	{ }
}

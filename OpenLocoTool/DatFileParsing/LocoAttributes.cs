namespace OpenLocoTool.DatFileParsing
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoArrayLengthAttribute(int length) : Attribute
	{
		public int Length => length;
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructOffsetAttribute(int offset) : Attribute
	{
		public int Offset => offset;
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class LocoStructSizeAttribute(int size) : Attribute
	{
		public int Size => size;
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class LocoStringTableAttribute(params string[] names) : Attribute
	{
		public string[] Names => names;

		public int Count => Names.Length;
	}

	// basically a 'skip' attribute to allow deferred loading for variable data
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructVariableLoadAttribute : Attribute
	{ }
}

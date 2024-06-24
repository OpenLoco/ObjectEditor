using OpenLoco.ObjectEditor.Data;

namespace OpenLoco.ObjectEditor.DatFileParsing
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoArrayLengthAttribute(uint length) : Attribute
	{
		public uint Length => length;
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructOffsetAttribute(uint offset) : Attribute
	{
		public uint Offset => offset;
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class LocoStructSizeAttribute(uint size) : Attribute
	{
		public uint Size => size;
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class LocoStructTypeAttribute(ObjectType objectType) : Attribute
	{
		public ObjectType ObjectType => objectType;
	}

	// basically a 'skip' attribute to allow deferred loading for variable data
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructVariableLoadAttribute : Attribute
	{ }

	// basically a 'skip' attribute to allow deferred loading for variable data, and writing of this property will be 0
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructSkipReadAttribute : Attribute
	{ }

	// basically a 'skip' attribute to allow deferred loading for variable data, and writing of this property will be 0
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStringAttribute : Attribute
	{ }

	// basically a 'skip' attribute to allow deferred loading for variable data, and writing of this property will be 0
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoImageIdAttribute : Attribute
	{ }

	// to mark properties that seemingly have no purpose or use
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoPropertyMaybeUnused : Attribute
	{ }

	// basically a 'skip' attribute to allow deferred loading for variable data, and writing of this property will be 0
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class LocoStringTableAttribute(params string[] strings) : Attribute
	{
		public string[] Strings => strings;
	}
}

namespace OpenLocoTool.DatFileParsing
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoArrayLengthAttribute : Attribute
	{
		public int Length { get; }

		public LocoArrayLengthAttribute(int length) => Length = length;
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
	public class LocoStructPropertyAttribute : Attribute
	{
		public LocoStructPropertyAttribute() { }
	}
}

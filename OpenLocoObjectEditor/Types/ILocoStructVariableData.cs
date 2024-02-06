using System.ComponentModel;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStructVariableData
	{
		ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData);

		ReadOnlySpan<byte> Save();
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStructPostLoad
	{
		void PostLoad();

		//ReadOnlySpan<byte> Save();
	}
}

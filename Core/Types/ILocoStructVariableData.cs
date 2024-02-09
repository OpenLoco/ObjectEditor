using System.ComponentModel;

namespace OpenLoco.ObjectEditor.DatFileParsing
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

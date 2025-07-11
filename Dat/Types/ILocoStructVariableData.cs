using System.ComponentModel;

namespace Dat.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
public interface ILocoStructVariableData
{
	ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData);

	ReadOnlySpan<byte> SaveVariable();
}

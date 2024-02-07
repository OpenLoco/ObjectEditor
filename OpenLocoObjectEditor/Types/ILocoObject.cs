using System.ComponentModel;
using OpenLocoObjectEditor.Headers;
using OpenLocoObjectEditor.Types;

namespace OpenLocoObjectEditor.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoObject
	{
		ILocoStruct Object { get; set; }
		StringTable StringTable { get; set; }
		List<G1Element32>? G1Elements { get; set; }
	}
}

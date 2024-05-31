using System.ComponentModel;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Types;

namespace OpenLoco.ObjectEditor.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoObject
	{
		ILocoStruct Object { get; set; }
		StringTable StringTable { get; set; }
		List<G1Element32> G1Elements { get; set; }
	}
}

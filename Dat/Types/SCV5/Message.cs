using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Types.SCV5;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0xD4)]
public class Message : ILocoStruct
{
	[LocoArrayLength(0xD4)] public uint8_t[] var_0 { get; set; }

	public bool Validate() => throw new NotImplementedException();
}

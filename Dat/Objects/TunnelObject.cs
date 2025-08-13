using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x06)]
[LocoStructType(DatObjectType.Tunnel)]
public record TunnelObject(
	[property: LocoStructOffset(0x00), Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), Browsable(false)] image_id Image
	) : ILocoStruct
{
	public bool Validate() => true;
}

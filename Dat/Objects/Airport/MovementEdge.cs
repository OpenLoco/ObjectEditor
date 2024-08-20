using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	public record MovementEdge(
		[property: LocoStructOffset(0x00)] uint8_t var_00,
		[property: LocoStructOffset(0x01)] uint8_t CurrNode,
		[property: LocoStructOffset(0x02)] uint8_t NextNode,
		[property: LocoStructOffset(0x03)] uint8_t var_03,
		[property: LocoStructOffset(0x04)] uint32_t MustBeClearEdges,    // Which edges must be clear to use the transition edge. should probably be some kind of flags?
		[property: LocoStructOffset(0x08)] uint32_t AtLeastOneClearEdges // Which edges must have at least one clear to use transition edge. should probably be some kind of flags?
		) : ILocoStruct
	{
		public bool Validate() => true;
	}
}

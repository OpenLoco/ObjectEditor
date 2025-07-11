using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x03)]
	public record SimpleAnimation(
		[property: LocoStructOffset(0x00), Browsable(false)] object_id ObjectId,
		[property: LocoStructOffset(0x01)] uint8_t Height,
		[property: LocoStructOffset(0x02)] SimpleAnimationType Type
		) : ILocoStruct
	{
		public SimpleAnimation() : this(0, 0, 0)
		{ }

		public bool Validate()
			=> true;
	}
}

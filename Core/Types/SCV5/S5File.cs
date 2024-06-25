using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;

namespace Core.Types.SCV5
{
	// todo: make a list? is this harcoded?
	public record S5File(
		[property: LocoStructOffset(0x00)] Header Header,
		[property: LocoStructOffset(0x20)] Options LandscapeOptions,
		[property: LocoStructOffset(0x20 + 0x431A)] SaveDetails SaveDetails
		//[property: LocoArrayLength(859)] ObjectHeader[] RequiredObjects,
		//[property: LocoStructOffset(0x00)] GameState GameState,
		//List<TileElement> TileElements, List<(ObjectHeader, byte[])> PackedObjects
		)
		: ILocoStruct
	{
		public bool Validate() => true;
	}
}

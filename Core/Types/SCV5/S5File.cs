using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;

namespace Core.Types.SCV5
{
	class S5File
	{
		[LocoStructOffset(0x00)] public Header Header { get; set; }
		[LocoStructOffset(0x20)] public Options? LandscapeOptions { get; set; }
		public SaveDetails? SaveDetails { get; set; }

		// todo: make a list? is this harcoded?
		[LocoArrayLength(859)]
		public ObjectHeader[] RequiredObjects { get; set; }
		public GameState GameState { get; set; }
		public List<TileElement> TileElements { get; set; }
		public List<(ObjectHeader, byte[])> PackedObjects { get; set; }
	}
}

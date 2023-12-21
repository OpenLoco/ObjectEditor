using OpenLocoTool.Headers;

namespace OpenLocoTool.DatFileParsing
{
	public class BaseObject
	{
		public ILocoStruct Object { get; set; }
		public StringTable StringTable { get; set; }
		public List<G1Element32> GraphicsTable { get; set; }
	}
}

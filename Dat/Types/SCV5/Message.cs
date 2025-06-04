using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	[LocoStructSize(0xD4)]
	public class Message : ILocoStruct
	{
		[LocoArrayLength(0xD4)] public uint8_t[] var_0 { get; set; }

		public bool Validate() => throw new NotImplementedException();
	}
}

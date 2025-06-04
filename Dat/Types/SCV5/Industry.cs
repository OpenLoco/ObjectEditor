using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	[LocoStructSize(0x453)]
	public class Industry : ILocoStruct
	{
		[LocoArrayLength(0x453)] public uint8_t[] var_0 { get; set; }

		public bool Validate() => throw new NotImplementedException();
	}
}

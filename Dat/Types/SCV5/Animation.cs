using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	[LocoStructSize(0x06)]
	public class Animation : ILocoStruct
	{
		[LocoArrayLength(0x06)] public uint8_t[] var_0 { get; set; } = [];

		public bool Validate()
			=> throw new NotImplementedException();
	};
}

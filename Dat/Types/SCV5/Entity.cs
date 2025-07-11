using Dat.FileParsing;

namespace Dat.Types.SCV5
{
	[LocoStructSize(0x80)]
	public class Entity : ILocoStruct
	{
		[LocoArrayLength(0x80)] public uint8_t[] var_0 { get; set; } = [];

		public bool Validate()
			=> throw new NotImplementedException();
	}
}

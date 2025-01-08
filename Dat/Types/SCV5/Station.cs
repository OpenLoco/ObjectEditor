using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	[LocoStructSize(0x3D2)]
	public class Station : ILocoStruct
	{
		[LocoArrayLength(0x3D2)] public uint8_t[] var_0 { get; set; }

		public bool Validate() => throw new NotImplementedException();
	};
}

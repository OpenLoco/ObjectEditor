namespace Definitions.ObjectModels.Objects.Sound;
public class SoundObject : ILocoStruct
{
	public uint8_t ShouldLoop { get; set; }
	public uint32_t Volume { get; set; }

	public bool Validate() => throw new NotImplementedException();
}

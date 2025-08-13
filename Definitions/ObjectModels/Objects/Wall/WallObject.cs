namespace Definitions.ObjectModels.Objects.Wall;

public class WallObject : ILocoStruct
{
	public uint8_t Height { get; set; }
	public WallObjectFlags1 Flags1 { get; set; } = WallObjectFlags1.None;
	public WallObjectFlags2 Flags2 { get; set; } = WallObjectFlags2.None;

	public bool Validate() => true;
}

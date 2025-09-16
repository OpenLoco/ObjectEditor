namespace Definitions.ObjectModels.Objects.HillShape;

public class HillShapesObject : ILocoStruct
{
	public uint8_t HillHeightMapCount { get; set; }
	public uint8_t MountainHeightMapCount { get; set; }
	public bool IsHeightMap { get; set; }

	public bool Validate() => true;
}

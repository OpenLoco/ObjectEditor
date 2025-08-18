using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class HillShapesObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[Flags]
internal enum DatHillShapeFlags : uint16_t
{
	None = 0,
	IsHeightMap = 1 << 0,
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x0E)]
[LocoStructType(DatObjectType.HillShapes)]
internal record DatHillShapesObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t HillHeightMapCount,
	[property: LocoStructOffset(0x03)] uint8_t MountainHeightMapCount,
	[property: LocoStructOffset(0x04), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x08), Browsable(false)] image_id ImageHill,
	[property: LocoStructOffset(0x0C)] DatHillShapeFlags Flags
	) : ILocoStruct
{
	public bool Validate() => true;
}

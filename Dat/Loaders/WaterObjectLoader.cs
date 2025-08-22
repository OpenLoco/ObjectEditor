using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Loaders;
public abstract class WaterObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream stream, LocoObject obj) => throw new NotImplementedException();
}

[LocoStructSize(0x0E)]
[LocoStructType(DatObjectType.Water)]
internal record DatWaterObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t CostIndex,
	[property: LocoStructOffset(0x03), LocoPropertyMaybeUnused] uint8_t var_03,
	[property: LocoStructOffset(0x04)] int16_t CostFactor,
	[property: LocoStructOffset(0x06), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x0A), Browsable(false)] image_id MapPixelImage
	)
{

}

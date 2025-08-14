using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class ScenarioTextObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[LocoStructSize(0x06)]
[LocoStructType(DatObjectType.ScenarioText)]
internal record ScenarioTextObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id Details,
	[property: LocoStructOffset(0x04), LocoArrayLength(0x6 - 0x4), Browsable(false)] uint8_t pad_04
	) : ILocoStruct
{
	public bool Validate() => true;
}

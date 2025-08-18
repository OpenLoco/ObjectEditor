using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class CliffEdgeObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream stream, LocoObject obj) => throw new NotImplementedException();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x06)]
[LocoStructType(DatObjectType.CliffEdge)]
internal record DatCliffEdgeObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoString, Browsable(false)] image_id Image
	)
{
	public bool Validate() => true;
}

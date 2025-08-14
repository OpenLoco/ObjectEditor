using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class TunnelObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[LocoStructSize(0x06)]
[LocoStructType(DatObjectType.Tunnel)]
internal record TunnelObject(
	[property: LocoStructOffset(0x00), Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), Browsable(false)] image_id Image
	)
{ }

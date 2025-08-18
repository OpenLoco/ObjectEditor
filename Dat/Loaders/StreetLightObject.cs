using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class StreetLightObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int DesignedYearLength = 3;
	}

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[LocoStructSize(0x0C)]
[LocoStructType(DatObjectType.StreetLight)]
public record DatStreetLightObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoArrayLength(StreetLightObjectLoader.Constants.DesignedYearLength)] uint16_t[] DesignedYears
)
{ }

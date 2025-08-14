using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.TownNames;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class TownNamesObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MinNumNameCombinations = 80;
	}

	public static class Sizes
	{
		public const int Category = 0x04; // 4 bytes
	}

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[LocoStructSize(0x1A)]
[LocoStructType(DatObjectType.TownNames)]
internal record TownNamesObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoArrayLength(6)] Category[] Categories
) : ILocoStruct, ILocoStructVariableData
{
	byte[] tempUnkVariableData;

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// town names is interesting - loco has not RE'd the whole object and there are no graphics, so we just
		// skip the rest of the data/object
		tempUnkVariableData = remainingData.ToArray();
		return remainingData[remainingData.Length..];
	}

	public ReadOnlySpan<byte> SaveVariable()
		=> tempUnkVariableData;

	public bool Validate() => true;
}

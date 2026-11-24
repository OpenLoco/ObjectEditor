// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Types;
using System.Text;

namespace Dat.Loaders;

public abstract class TownNamesObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MinNumNameCombinations = 80;
		public const int Categories = 6;
	}

	public static class StructSizes
	{
		public const int Category = 0x1A;
	}

	public static ObjectType ObjectType => ObjectType.TownNames;
	public static DatObjectType DatObjectType => DatObjectType.TownNames;

	public static LocoObject Load(Stream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TownNamesObject();

			// fixed
			br.SkipStringId();

			var morphemeCategories = new DatMorphemeCategory[Constants.Categories];
			for (var i = 0; i < Constants.Categories; ++i)
			{
				morphemeCategories[i] = new DatMorphemeCategory(
					Count: br.ReadByte(),
					Bias: br.ReadByte(),
					Offset: br.ReadUInt16()
				);
			}

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable

			// read it all since we need to do index/offset lookups
			var strTableBasePtr = br.BaseStream.Position;
			var extra = br.ReadToEnd();

			foreach (var c in morphemeCategories)
			{
				if (c.Count == 0)
				{
					model.MorphemeCategories.Add(
						new MorphemeCategory()
						{
							DatCount = 0,
							Bias = 0,
							DatOffset = 0,
							TownNames = []
						});

					continue;
				}

				var ptr = (int)(c.Offset - strTableBasePtr);
				model.MorphemeCategories.Add(
					new MorphemeCategory()
					{
						DatCount = c.Count,
						Bias = c.Bias,
						DatOffset = c.Offset,
						TownNames = [.. TownNamesStringTableReader.ReadStringTable(extra[ptr..], c.Count)]
					});
			}

			// image table
			// N/A

			return new LocoObject(ObjectType, model, stringTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var model = obj.Object as TownNamesObject;

		using var stringTableStream = new MemoryStream();
		SawyerStreamWriter.WriteStringTable(stringTableStream, obj.StringTable);
		var stringTableBytes = stringTableStream.ToArray();

		// need to first serialise the stringtable to get the offsets for the morpheme categories
		var (data, categories) = TownNamesStringTableWriter.WriteStringTable(model.MorphemeCategories);
		var variableDataBaseOffset = StructSizes.Category + stringTableBytes.Length;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId();// Name offset, not part of object definition
			for (var i = 0; i < Constants.Categories; ++i)
			{
				var category = model.MorphemeCategories[i];
				var count = (uint8_t)category.TownNames.Count;
				category.DatCount = count;
				bw.Write(count);
				bw.Write(category.Bias);
				bw.Write((ushort)(variableDataBaseOffset + categories[i].Offset));
			}

			// string table
			stream.Write(stringTableBytes);

			// variable
			// N/A
			bw.Write(data);
		}

		// image table
		// N/A
	}
}

internal record DatMorphemeCategory(
	uint8_t Count,
	uint8_t Bias,
	uint16_t Offset);

[Flags]
internal enum DatLocationFlags : uint8_t
{
	None = 0,
	AdjacentToLargeWaterBody = 1 << 0,
	NotMountainous = 1 << 1,
	AdjacentToSmallWaterBody = 1 << 2,
};

internal static class LocationFlagsConverter
{
	public static LocationFlags Convert(this DatLocationFlags datLocationFlags)
		=> (LocationFlags)datLocationFlags;

	public static DatLocationFlags Convert(this LocationFlags locationFlags)
		=> (DatLocationFlags)locationFlags;
}

internal static class TownNamesStringTableReader
{
	public static StringTableEntry[] ReadStringTable(byte[] data, int numEntries)
		=> ReadStringTable(data, 0, numEntries, out var entries) > 0
			? entries
			: [];

	public static int ReadStringTable(byte[] data, int id, int numEntries, out StringTableEntry[] entries)
	{
		entries = new StringTableEntry[numEntries];
		var consumed = numEntries * 2;

		for (var i = 0; i < numEntries; i++)
		{
			int ptr = BitConverter.ToUInt16(data, i * 2);

			var end = ptr;
			while (end < data.Length && data[end] != 0)
			{
				end++;
			}

			var text = Encoding.Latin1.GetString(data, ptr, end - ptr);
			var type = data[end + 1];

			entries[i] = new StringTableEntry(text, ((DatLocationFlags)type).Convert());

			// Each string contributes: offset + content length + null terminator + type byte
			consumed = ptr + (end - ptr) + 2;
		}

		return consumed;
	}
}

internal static class TownNamesStringTableWriter
{
	public static (byte[] Data, IReadOnlyList<DatMorphemeCategory> Categories) WriteStringTable(List<MorphemeCategory> categories)
	{
		var categoryInfoList = new List<DatMorphemeCategory>();
		var categoryDataSizes = new int[categories.Count];
		var totalSize = 0;

		// First pass: calculate total size and category offsets
		for (var c = 0; c < categories.Count; c++)
		{
			var category = categories[c];
			var count = category.TownNames.Count;
			var offsetTableSize = count * 2;
			var stringDataSize = 0;

			for (var i = 0; i < count; i++)
			{
				var encodedBytes = Encoding.Latin1.GetBytes(category.TownNames[i].Text);
				stringDataSize += encodedBytes.Length + 2; // +1 for null, +1 for location hint
			}

			categoryDataSizes[c] = offsetTableSize + stringDataSize;
			categoryInfoList.Add(new DatMorphemeCategory((uint8_t)count, category.Bias, (uint16_t)totalSize));
			totalSize += categoryDataSizes[c];
		}

		var result = new byte[totalSize];
		var ptr = 0;

		// Second pass: write each category
		for (var c = 0; c < categories.Count; c++)
		{
			var category = categories[c];
			var categoryStartPtr = ptr;
			var offsetTableSize = category.TownNames.Count * 2;
			ptr += offsetTableSize;

			for (var i = 0; i < category.TownNames.Count; i++)
			{
				var entry = category.TownNames[i];

				// Write offset (relative to category start)
				var offset = (ushort)(ptr - categoryStartPtr);
				BitConverter.GetBytes(offset).CopyTo(result, categoryStartPtr + i * 2);

				var encodedBytes = Encoding.Latin1.GetBytes(entry.Text);
				Array.Copy(encodedBytes, 0, result, ptr, encodedBytes.Length);
				ptr += encodedBytes.Length;

				result[ptr++] = 0; // null terminator
				result[ptr++] = (byte)entry.LocationHint;
			}
		}

		return new(result, categoryInfoList);
	}

}

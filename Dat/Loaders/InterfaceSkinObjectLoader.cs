using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.InterfaceSkin;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class InterfaceSkinObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x18;
	}

	public static ObjectType ObjectType => ObjectType.InterfaceSkin;
	public static DatObjectType DatObjectType => DatObjectType.InterfaceSkin;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new InterfaceSkinObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image offset, not part of object definition
			model.MapTooltipObjectColour = (Colour)br.ReadByte();
			model.MapTooltipCargoColour = (Colour)br.ReadByte();
			model.TooltipColour = (Colour)br.ReadByte();
			model.ErrorColour = (Colour)br.ReadByte();
			model.WindowPlayerColour = (Colour)br.ReadByte();
			model.WindowTitlebarColour = (Colour)br.ReadByte();
			model.WindowColour = (Colour)br.ReadByte();
			model.WindowConstructionColour = (Colour)br.ReadByte();
			model.WindowTerraFormColour = (Colour)br.ReadByte();
			model.WindowMapColour = (Colour)br.ReadByte();
			model.WindowOptionsColour = (Colour)br.ReadByte();
			model.Colour_11 = (Colour)br.ReadByte();
			model.TopToolbarPrimaryColour = (Colour)br.ReadByte();
			model.TopToolbarSecondaryColour = (Colour)br.ReadByte();
			model.TopToolbarTertiaryColour = (Colour)br.ReadByte();
			model.TopToolbarQuaternaryColour = (Colour)br.ReadByte();
			model.PlayerInfoToolbarColour = (Colour)br.ReadByte();
			model.TimeToolbarColour = (Colour)br.ReadByte();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (InterfaceSkinObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write((uint8_t)model.MapTooltipObjectColour);
			bw.Write((uint8_t)model.MapTooltipCargoColour);
			bw.Write((uint8_t)model.TooltipColour);
			bw.Write((uint8_t)model.ErrorColour);
			bw.Write((uint8_t)model.WindowPlayerColour);
			bw.Write((uint8_t)model.WindowTitlebarColour);
			bw.Write((uint8_t)model.WindowColour);
			bw.Write((uint8_t)model.WindowConstructionColour);
			bw.Write((uint8_t)model.WindowTerraFormColour);
			bw.Write((uint8_t)model.WindowMapColour);
			bw.Write((uint8_t)model.WindowOptionsColour);
			bw.Write((uint8_t)model.Colour_11);
			bw.Write((uint8_t)model.TopToolbarPrimaryColour);
			bw.Write((uint8_t)model.TopToolbarSecondaryColour);
			bw.Write((uint8_t)model.TopToolbarTertiaryColour);
			bw.Write((uint8_t)model.TopToolbarQuaternaryColour);
			bw.Write((uint8_t)model.PlayerInfoToolbarColour);
			bw.Write((uint8_t)model.TimeToolbarColour);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}
}

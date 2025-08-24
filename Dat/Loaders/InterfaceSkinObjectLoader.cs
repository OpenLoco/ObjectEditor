using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.InterfaceSkin;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class InterfaceSkinObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x18;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new InterfaceSkinObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image offset, not part of object definition
			model.MapTooltipObjectColour = (Colour)br.ReadByte();
			model.MapTooltipCargoColour = (Colour)br.ReadByte();
			model.TooltipColour = (Colour)br.ReadByte();
			model.ErrorColour = (Colour)br.ReadByte();
			model.WindowPlayerColor = (Colour)br.ReadByte();
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
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.InterfaceSkin), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.InterfaceSkin, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
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
			bw.Write((uint8_t)model.WindowPlayerColor);
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
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}
}

[LocoStructSize(0x18)]
[LocoStructType(DatObjectType.InterfaceSkin)]
internal record DatInterfaceSkinObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x06)] DatColour MapTooltipObjectColour,
	[property: LocoStructOffset(0x07)] DatColour MapTooltipCargoColour,
	[property: LocoStructOffset(0x08)] DatColour TooltipColour,
	[property: LocoStructOffset(0x09)] DatColour ErrorColour,
	[property: LocoStructOffset(0x0A)] DatColour WindowPlayerColor,
	[property: LocoStructOffset(0x0B)] DatColour WindowTitlebarColour,
	[property: LocoStructOffset(0x0C)] DatColour WindowColour,
	[property: LocoStructOffset(0x0D)] DatColour WindowConstructionColour,
	[property: LocoStructOffset(0x0E)] DatColour WindowTerraFormColour,
	[property: LocoStructOffset(0x0F)] DatColour WindowMapColour,
	[property: LocoStructOffset(0x10)] DatColour WindowOptionsColour,
	[property: LocoStructOffset(0x11)] DatColour Colour_11,
	[property: LocoStructOffset(0x12)] DatColour TopToolbarPrimaryColour,
	[property: LocoStructOffset(0x13)] DatColour TopToolbarSecondaryColour,
	[property: LocoStructOffset(0x14)] DatColour TopToolbarTertiaryColour,
	[property: LocoStructOffset(0x15)] DatColour TopToolbarQuaternaryColour,
	[property: LocoStructOffset(0x16)] DatColour PlayerInfoToolbarColour,
	[property: LocoStructOffset(0x17)] DatColour TimeToolbarColour
	)
{ }

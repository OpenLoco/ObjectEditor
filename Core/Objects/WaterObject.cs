
using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Types;

namespace OpenLoco.ObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0E)]
	[LocoStructType(ObjectType.Water)]
	[LocoStringTable("Name")]
	public record WaterObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t CostIndex,
		[property: LocoStructOffset(0x03), LocoPropertyMaybeUnused] uint8_t var_03,
		[property: LocoStructOffset(0x04)] int8_t CostFactor,
		[property: LocoStructOffset(0x05), LocoPropertyMaybeUnused] uint8_t var_05,
		[property: LocoStructOffset(0x06), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x0A), Browsable(false)] image_id MapPixelImage
		) : ILocoStruct, ILocoImageTableNames
	{
		public bool Validate()
		{
			if (CostIndex > 32)
			{
				return false;
			}

			if (CostFactor <= 0)
			{
				return false;
			}

			return true;
		}

		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public static Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 42, "kToolbarTerraformWater" },
		};
	}
}

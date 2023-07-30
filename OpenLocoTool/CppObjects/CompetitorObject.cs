using System.ComponentModel;

namespace OpenLocoTool.Objects
{
	// size = 0x38
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CompetitorObject(
		string var_00,           // 0x00
		string var_02,           // 0x02
		uint32_t var_04,         // 0x04
		uint32_t var_08,         // 0x08
		uint32_t Emotions,       // 0x0C
		uint32_t[] Images,       // 0x10
		uint8_t Intelligence,    // 0x34
		uint8_t Aggressiveness,  // 0x35
		uint8_t Competitiveness, // 0x36
		uint8_t var_37)          // 0x37
	{
		public static CompetitorObject Read(ReadOnlySpan<byte> data)
		{
			var var_00 = "todo: implement code to lookup string table";
			var var_02 = "todo: implement code to lookup string table";
			var var_04 = BitConverter.ToUInt32(data[4..8]);
			var var_08 = BitConverter.ToUInt32(data[8..12]);
			var emotions = BitConverter.ToUInt32(data[12..16]);

			// images
			var images = new uint32_t[9];
			for (var i = 0; i < images.Length; i++)
			{
				images[i] = BitConverter.ToUInt32(data[(16 + (i * 4))..(20 + (i * 4))]);
			}

			var intelligence = data[0x34];
			var aggressiveness = data[0x35];
			var competetiveness = data[0x36];
			var var_37 = data[0x37];

			return new CompetitorObject(var_00, var_02, var_04, var_08, emotions, images, intelligence, aggressiveness, competetiveness, var_37);
		}
	}

}

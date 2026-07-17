using Definitions.ObjectModels.Objects.Sound;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectSound : DbSubObject, IConvertibleToTable<TblObjectSound, SoundObject>
{
	public uint8_t ShouldLoop { get; set; }
	public uint32_t Volume { get; set; }
	public string SoundObjectData { get; set; } = "null";
	public byte[] PcmData { get; set; } = [];
	public byte[] UnkData { get; set; } = [];

	public static TblObjectSound FromObject(TblObject tbl, SoundObject obj)
		=> new()
		{
			Parent = tbl,
			ShouldLoop = obj.ShouldLoop,
			Volume = obj.Volume,
			SoundObjectData = JsonSerializer.Serialize(obj.SoundObjectData),
			PcmData = obj.PcmData,
			UnkData = obj.UnkData,
		};
}
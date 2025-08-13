using Definitions.ObjectModels.Objects.Sound;

namespace Definitions.Database;

public class TblObjectSound : DbSubObject, IConvertibleToTable<TblObjectSound, SoundObject>
{
	public uint8_t ShouldLoop { get; set; }
	public uint32_t Volume { get; set; }

	public static TblObjectSound FromObject(TblObject tbl, SoundObject obj)
		=> new()
		{
			Parent = tbl,
			ShouldLoop = obj.ShouldLoop,
			Volume = obj.Volume,
		};
}

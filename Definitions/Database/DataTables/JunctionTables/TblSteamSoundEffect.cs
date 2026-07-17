namespace Definitions.Database;

public class TblSteamSoundEffect
{
	public UniqueObjectId SteamId { get; set; }
	public required TblObjectSteam Steam { get; set; }

	public UniqueObjectId SoundId { get; set; }
	public required TblObjectSound Sound { get; set; }
}

namespace Dat.Data;

public enum SourceGame : byte
{
	Custom = 0,
	//DataFile = 1,
	Vanilla = 2,
	OpenLoco = 3,
}
public enum ObjectSource // note this is similar to the DAT enum `SourceGame`, but we need more definition now
{
	Custom,
	LocomotionSteam,
	LocomotionGoG,
	OpenLoco,
}

public static class ObjectSourceConverter
{
	public static SourceGame ToSourceGame(this ObjectSource objectSource)
		=> objectSource switch
		{
			ObjectSource.Custom => SourceGame.Custom,
			ObjectSource.LocomotionSteam => SourceGame.Vanilla,
			ObjectSource.LocomotionGoG => SourceGame.Vanilla,
			ObjectSource.OpenLoco => SourceGame.OpenLoco,
			_ => throw new NotImplementedException(),
		};
}

using Dat.Data;
using Definitions.ObjectModels.Types;

namespace Dat.Converters;

public static class ObjectSourceConverter
{
	public static DatObjectSource Convert(this ObjectSource objectSource)
		=> objectSource switch
		{
			ObjectSource.Custom => DatObjectSource.Custom,
			ObjectSource.LocomotionSteam => DatObjectSource.Vanilla,
			ObjectSource.LocomotionGoG => DatObjectSource.Vanilla,
			ObjectSource.OpenLoco => DatObjectSource.OpenLoco,
			_ => throw new NotImplementedException(),
		};

	public static ObjectSource Convert(this DatObjectSource objectSource)
		=> objectSource switch
		{
			DatObjectSource.Custom => ObjectSource.Custom,
			DatObjectSource.Vanilla => ObjectSource.LocomotionSteam, // todo - can check the file checksum to determine if it's GoG or Steam
			DatObjectSource.OpenLoco => ObjectSource.OpenLoco,
			_ => throw new NotImplementedException(),
		};
}

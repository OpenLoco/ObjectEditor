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

	public static ObjectSource Convert(this DatObjectSource objectSource, string datName, uint32_t datChecksum)
		=> objectSource switch
		{
			DatObjectSource.Custom => ObjectSource.Custom,
			DatObjectSource.Vanilla => OriginalObjectFiles.GetFileSource(datName, datChecksum, objectSource),
			DatObjectSource.OpenLoco => ObjectSource.OpenLoco,
			_ => throw new NotImplementedException(),
		};
}

using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Dat.Types.SCV5;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
using static Dat.Loaders.BuildingObjectLoader;
using static Dat.Loaders.VehicleObjectLoader;

namespace Dat.Loaders;

public abstract partial class VehicleObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int CompatibleVehicleCount = 8;
		public const int RequiredTrackExtrasCount = 4;
		public const int CarComponentsCount = 4;
		public const int AnimationCount = 2;
		public const int CompatibleCargoTypesLength = 2;
		public const int CargoTypeSpriteOffsetsLength = 32;
		public const int MaxUnionSoundStructLength = 0x1B;
		public const int MaxBodySprites = 4;
		public const int MaxBogieSprites = 2;
		public const int MaxStartSounds = 3;
	}

	public static class StructSizes
	{
		public const int Dat = 0x15E;
		public const int SoundData = 0x1B;
		public const int FrictionSound = 0x08;
		public const int SimpleMotorSound = 0x11;
		public const int GearboxMotorSound = 0x1B;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new VehicleObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.Mode = ((DatTransportMode)br.ReadByte()).Convert();
			model.Type = ((DatVehicleType)br.ReadByte()).Convert();
			model.NumCarComponents = br.ReadByte();
			br.SkipObjectId(); // TrackTypeId, not part of object definition
			var numRequiredTrackExtras = br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.CostFactor = br.ReadInt16();
			model.Reliability = br.ReadByte();
			model.RunCostIndex = br.ReadByte();
			model.RunCostFactor = br.ReadInt16();
			model.SpecialColourSchemeIndex = ((DatCompanyColourType)br.ReadByte()).Convert();
			var numCompatibleVehicles = br.ReadByte();
			br.SkipUInt16(Constants.CompatibleVehicleCount);
			br.SkipByte(Constants.RequiredTrackExtrasCount);
			model.CarComponents = br.ReadCarComponents(Constants.CarComponentsCount);
			model.BodySprites = br.ReadBodySprites(Constants.MaxBodySprites);
			model.BogieSprites = br.ReadBogieSprites(Constants.MaxBogieSprites);
			model.Power = br.ReadUInt16();
			model.Speed = br.ReadInt16();
			model.RackSpeed = br.ReadInt16();
			model.Weight = br.ReadUInt16();
			model.Flags = ((DatVehicleObjectFlags)br.ReadUInt16()).Convert();
			br.SkipByte(Constants.CompatibleCargoTypesLength * 1); // MaxCargo, read in LoadVariable
			br.SkipByte(Constants.CompatibleCargoTypesLength * 4); // CompatibleCargoCategories, read in LoadVariable
			br.SkipByte(Constants.CargoTypeSpriteOffsetsLength * 1); // CargoTypeSpriteOffsets, read in LoadVariable
			br.SkipByte(); // NumSimultaneousCargoTypes, manipulated in LoadVariable
			model.Animation = br.ReadSimpleAnimations(Constants.AnimationCount);
			model.ShipWakeOffset = br.ReadByte(); // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			br.SkipObjectId(); // RackRailType, not part of object definition
			model.DrivingSoundType = ((DatDrivingSoundType)br.ReadByte()).Convert();

			switch (model.DrivingSoundType)
			{
				case DrivingSoundType.Friction:
					model.FrictionSound = br.ReadFrictionSound();
					br.SkipByte(StructSizes.SoundData - StructSizes.FrictionSound);
					break;
				case DrivingSoundType.SimpleMotor:
					model.SimpleMotorSound = br.ReadSimpleMotorSound();
					br.SkipByte(StructSizes.SoundData - StructSizes.SimpleMotorSound);
					break;
				case DrivingSoundType.GearboxMotor:
					model.GearboxMotorSound = br.ReadGearboxMotorSound();
					br.SkipByte(StructSizes.SoundData - StructSizes.GearboxMotorSound);
					break;
				case DrivingSoundType.None:
					// nothing to read
					br.SkipByte(StructSizes.SoundData);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(model.DrivingSoundType), model.DrivingSoundType, null);
			}

			model.var_135 = br.ReadBytes(0x15A - 0x135);
			var numStartSounds = br.ReadByte();
			br.SkipByte(Constants.MaxStartSounds); // StartSounds, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Vehicle), null);

			// variable
			{
				// track type
				if (!model.Flags.HasFlag(VehicleObjectFlags.AnyRoadType) && (model.Mode == TransportMode.Rail || model.Mode == TransportMode.Road))
				{
					model.TrackType = br.ReadS5Header();
				}

				model.RequiredTrackExtras = br.ReadS5HeaderList(numRequiredTrackExtras);

				// compatible cargo
				for (var i = 0; i < Constants.CompatibleCargoTypesLength; ++i)
				{
					model.CompatibleCargoCategories.Add([]);
					var index = model.NumSimultaneousCargoTypes;
					model.MaxCargo.Add(br.ReadByte());

					if (model.MaxCargo[index] == 0)
					{
						continue;
					}

					while ((CargoCategory)br.PeekUInt16() != CargoCategory.NULL)
					{
						var cargoCategory = (CargoCategory)br.ReadUInt16();
						var cargoTypeSpriteOffset = br.ReadByte();

						model.CompatibleCargoCategories[index].Add(cargoCategory);

						if (!model.CargoTypeSpriteOffsets.TryAdd(cargoCategory, cargoTypeSpriteOffset))
						{
							// invalid object - shouldn't have 2 cargo types that are the same
						}
					}

					br.SkipByte(2); // skips the 0xFFFF bytes

					if (model.CompatibleCargoCategories[index].Count == 0)
					{
						model.MaxCargo[index] = 0;
					}
					else
					{
						model.NumSimultaneousCargoTypes++;
					}
				}

				// animation
				foreach (var anim in model.Animation)
				{
					if (anim.Type == SimpleAnimationType.None)
					{
						continue;
					}

					model.AnimationHeaders.Add(br.ReadS5Header());
				}

				model.CompatibleVehicles = br.ReadS5HeaderList(numCompatibleVehicles);

				// rack rail
				if (model.Flags.HasFlag(VehicleObjectFlags.RackRail))
				{
					model.RackRail = br.ReadS5Header();
				}

				// driving sound
				if (model.DrivingSoundType != DrivingSoundType.None)
				{
					model.Sound = br.ReadS5Header();
				}

				// driving start sounds
				const int mask = 127;
				var count = numStartSounds & mask;
				model.StartSounds = br.ReadS5HeaderList(count);
			}

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Vehicle, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition

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

internal static class SimpleAnimationTypeConverter
{
	public static SimpleAnimationType Convert(this DatSimpleAnimationType type)
		=> type switch
		{
			DatSimpleAnimationType.None => SimpleAnimationType.None,
			DatSimpleAnimationType.SteamPuff1 => SimpleAnimationType.SteamPuff1,
			DatSimpleAnimationType.SteamPuff2 => SimpleAnimationType.SteamPuff2,
			DatSimpleAnimationType.SteamPuff3 => SimpleAnimationType.SteamPuff3,
			DatSimpleAnimationType.DieselExhaust1 => SimpleAnimationType.DieselExhaust1,
			DatSimpleAnimationType.ElectricSpark1 => SimpleAnimationType.ElectricSpark1,
			DatSimpleAnimationType.ElectricSpark2 => SimpleAnimationType.ElectricSpark2,
			DatSimpleAnimationType.DieselExhaust2 => SimpleAnimationType.DieselExhaust2,
			DatSimpleAnimationType.ShipWake => SimpleAnimationType.ShipWake,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
		};

	public static DatSimpleAnimationType Convert(this SimpleAnimationType type)
		=> type switch
		{
			SimpleAnimationType.None => DatSimpleAnimationType.None,
			SimpleAnimationType.SteamPuff1 => DatSimpleAnimationType.SteamPuff1,
			SimpleAnimationType.SteamPuff2 => DatSimpleAnimationType.SteamPuff2,
			SimpleAnimationType.SteamPuff3 => DatSimpleAnimationType.SteamPuff3,
			SimpleAnimationType.DieselExhaust1 => DatSimpleAnimationType.DieselExhaust1,
			SimpleAnimationType.ElectricSpark1 => DatSimpleAnimationType.ElectricSpark1,
			SimpleAnimationType.ElectricSpark2 => DatSimpleAnimationType.ElectricSpark2,
			SimpleAnimationType.DieselExhaust2 => DatSimpleAnimationType.DieselExhaust2,
			SimpleAnimationType.ShipWake => DatSimpleAnimationType.ShipWake,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
		};
}

internal static class TransportModeConverter
{
	public static TransportMode Convert(this DatTransportMode datTransportMode)
		=> (TransportMode)datTransportMode;

	public static DatTransportMode Convert(this TransportMode transportMode)
		=> (DatTransportMode)transportMode;
}

internal static class VehicleTypeConverter
{
	public static VehicleType Convert(this DatVehicleType datVehicleType)
		=> (VehicleType)datVehicleType;

	public static DatVehicleType Convert(this VehicleType vehicleType)
		=> (DatVehicleType)vehicleType;
}

internal static class CompanyColourTypeConverter
{
	public static CompanyColourType Convert(this DatCompanyColourType datCompanyColourType)
		=> (CompanyColourType)datCompanyColourType;

	public static DatCompanyColourType Convert(this CompanyColourType companyColourType)
		=> (DatCompanyColourType)companyColourType;
}

internal static class VehicleObjectFlagsConverter
{
	public static VehicleObjectFlags Convert(this DatVehicleObjectFlags datVehicleObjectFlags)
		=> (VehicleObjectFlags)datVehicleObjectFlags;

	public static DatVehicleObjectFlags Convert(this VehicleObjectFlags vehicleObjectFlags)
		=> (DatVehicleObjectFlags)vehicleObjectFlags;
}

internal static class DrivingSoundTypeConverter
{
	public static DrivingSoundType Convert(this DatDrivingSoundType datDrivingSoundType)
		=> (DrivingSoundType)datDrivingSoundType;

	public static DatDrivingSoundType Convert(this DrivingSoundType drivingSoundType)
		=> (DatDrivingSoundType)drivingSoundType;
}

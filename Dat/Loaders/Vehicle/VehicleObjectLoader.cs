using Common;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.VehicleObjectLoader;

namespace Dat.Loaders;

public abstract partial class VehicleObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int CompatibleVehicleCount = 8;
		public const int RequiredTrackExtrasCount = 4;
		public const int MaxCarComponents = 4;
		public const int MaxCompatibleCargoCategories = 2;
		public const int CargoTypeSpriteOffsetsLength = 32;
		public const int MaxUnionSoundStructLength = 0x1B;
		public const int MaxBodySprites = 4;
		public const int MaxBogieSprites = 2;
		public const int MaxStartSounds = 3;
		public const int MaxSimpleAnimations = 2;
		public const int Var135PadSize = 0x15A - 0x135;
	}

	public static class StructSizes
	{
		//public const int Dat = 0x15E;
		public const int SoundData = 0x1B;
		public const int FrictionSound = 0x0B;
		public const int SimpleMotorSound = 0x11;
		public const int GearboxMotorSound = 0x1B;
	}

	public static ObjectType ObjectType => ObjectType.Vehicle;
	public static DatObjectType DatObjectType => DatObjectType.Vehicle;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new VehicleObject();

			// fixed
			LoadFixed(br, model, out var numRequiredTrackExtras, out var numCompatibleVehicles, out var numStartSounds);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			LoadVariable(br, model, numRequiredTrackExtras, numCompatibleVehicles, numStartSounds);

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableLoader.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, VehicleObject model, byte numRequiredTrackExtras, byte numCompatibleVehicles, byte numStartSounds)
	{
		// track type
		if (!model.Flags.HasFlag(VehicleObjectFlags.AnyRoadType) && (model.Mode == TransportMode.Rail || model.Mode == TransportMode.Road))
		{
			model.TrackType = br.ReadS5Header();
		}

		// required track extra
		model.RequiredTrackExtras = br.ReadS5HeaderList(numRequiredTrackExtras).ToArray();

		// compatible cargo
		for (var i = 0; i < Constants.MaxCompatibleCargoCategories; ++i)
		{
			model.CompatibleCargoCategories[i] = [];
			var index = model.NumSimultaneousCargoTypes;
			model.MaxCargo[index] = br.ReadByte();

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

		// compatible vehicles
		model.CompatibleVehicles = br.ReadS5HeaderList(numCompatibleVehicles).ToArray();

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
		model.StartSounds = br.ReadS5HeaderList(count).ToArray();
	}

	private static void LoadFixed(LocoBinaryReader br, VehicleObject model, out byte numRequiredTrackExtras, out byte numCompatibleVehicles, out byte numStartSounds)
	{
		br.SkipStringId(); // Name offset, not part of object definition
		model.Mode = ((DatTransportMode)br.ReadByte()).Convert();
		model.Type = ((DatVehicleType)br.ReadByte()).Convert();
		model.NumCarComponents = br.ReadByte();
		br.SkipObjectId(); // TrackTypeId, not part of object definition
		numRequiredTrackExtras = br.ReadByte();
		model.CostIndex = br.ReadByte();
		model.CostFactor = br.ReadInt16();
		model.Reliability = br.ReadByte();
		model.RunCostIndex = br.ReadByte();
		model.RunCostFactor = br.ReadInt16();
		model.SpecialColourSchemeIndex = ((DatCompanyColourType)br.ReadByte()).Convert();
		numCompatibleVehicles = br.ReadByte();
		br.SkipUInt16(Constants.CompatibleVehicleCount);
		br.SkipByte(Constants.RequiredTrackExtrasCount);
		model.CarComponents = br.ReadCarComponents(Constants.MaxCarComponents);
		model.BodySprites = br.ReadBodySprites(Constants.MaxBodySprites);
		model.BogieSprites = br.ReadBogieSprites(Constants.MaxBogieSprites);
		model.Power = br.ReadUInt16();
		model.Speed = br.ReadInt16();
		model.RackSpeed = br.ReadInt16();
		model.Weight = br.ReadUInt16();
		model.Flags = ((DatVehicleObjectFlags)br.ReadUInt16()).Convert();
		br.SkipByte(Constants.MaxCompatibleCargoCategories * 1); // MaxCargo, read in LoadVariable
		br.SkipByte(Constants.MaxCompatibleCargoCategories * 4); // CompatibleCargoCategories, read in LoadVariable
		br.SkipByte(Constants.CargoTypeSpriteOffsetsLength * 1); // CargoTypeSpriteOffsets, read in LoadVariable
		br.SkipByte(); // NumSimultaneousCargoTypes, manipulated in LoadVariable
		model.Animation = br.ReadSimpleAnimations(Constants.MaxSimpleAnimations);
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
				br.SkipByte(StructSizes.SoundData);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(model.DrivingSoundType), model.DrivingSoundType, null);
		}

		model.var_135 = br.ReadBytes(0x15A - 0x135);
		numStartSounds = br.ReadByte();
		br.SkipByte(Constants.MaxStartSounds); // StartSounds, not part of object definition
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (VehicleObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write((uint8_t)model.Mode.Convert());
			bw.Write((uint8_t)model.Type.Convert());
			bw.Write(model.NumCarComponents);
			bw.WriteEmptyObjectId(); // TrackTypeId, not part of object definition
			bw.Write((uint8_t)model.RequiredTrackExtras.Length);
			bw.Write(model.CostIndex);
			bw.Write(model.CostFactor);
			bw.Write(model.Reliability);
			bw.Write(model.RunCostIndex);
			bw.Write(model.RunCostFactor);
			bw.Write((uint8_t)model.SpecialColourSchemeIndex.Convert());
			bw.Write((uint8_t)model.CompatibleVehicles.Length);
			bw.WriteEmptyBytes(Constants.CompatibleVehicleCount * 2);
			bw.WriteEmptyBytes(Constants.RequiredTrackExtrasCount);
			bw.Write(model.CarComponents.Fill(Constants.MaxCarComponents, new VehicleObjectCar()).ToArray());
			bw.Write(model.BodySprites.Fill(Constants.MaxBodySprites, new BodySprite()).ToArray());
			bw.Write(model.BogieSprites.Fill(Constants.MaxBogieSprites, new BogieSprite()).ToArray());
			bw.Write(model.Power);
			bw.Write(model.Speed);
			bw.Write(model.RackSpeed);
			bw.Write(model.Weight);
			bw.Write((uint16_t)model.Flags.Convert());
			bw.WriteEmptyBytes(Constants.MaxCompatibleCargoCategories * 1); // MaxCargo, read in LoadVariable
			bw.WriteEmptyBytes(Constants.MaxCompatibleCargoCategories * 4); // CompatibleCargoCategories, read in LoadVariable
			bw.WriteEmptyBytes(Constants.CargoTypeSpriteOffsetsLength * 1); // CargoTypeSpriteOffsets, read in LoadVariable
			bw.WriteEmptyBytes(1); // NumSimultaneousCargoTypes, manipulated in LoadVariable
			bw.Write(model.Animation.Fill(Constants.MaxSimpleAnimations, new SimpleAnimation()).ToArray());
			bw.Write(model.ShipWakeOffset); // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.WriteEmptyObjectId(); // RackRailType, not part of object definition
			bw.Write((uint8_t)model.DrivingSoundType.Convert());

			// sound union
			switch (model.DrivingSoundType)
			{
				case DrivingSoundType.Friction:
					ArgumentNullException.ThrowIfNull(model.FrictionSound);
					bw.Write(model.FrictionSound);
					bw.WriteEmptyBytes(StructSizes.SoundData - StructSizes.FrictionSound);
					break;
				case DrivingSoundType.SimpleMotor:
					ArgumentNullException.ThrowIfNull(model.SimpleMotorSound);
					bw.Write(model.SimpleMotorSound);
					bw.WriteEmptyBytes(StructSizes.SoundData - StructSizes.SimpleMotorSound);
					break;
				case DrivingSoundType.GearboxMotor:
					ArgumentNullException.ThrowIfNull(model.GearboxMotorSound);
					bw.Write(model.GearboxMotorSound);
					bw.WriteEmptyBytes(StructSizes.SoundData - StructSizes.GearboxMotorSound);
					break;
				case DrivingSoundType.None:
					bw.WriteEmptyBytes(StructSizes.SoundData);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(model.DrivingSoundType), model.DrivingSoundType, null);
			}

			bw.Write(model.var_135.Fill(Constants.Var135PadSize, (byte)0).ToArray());
			bw.Write((uint8_t)model.StartSounds.Length);
			bw.WriteEmptyBytes(Constants.MaxStartSounds * 1); // StartSounds, not part of object

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	private static void SaveVariable(VehicleObject model, LocoBinaryWriter bw)
	{
		// track type
		if (!model.Flags.HasFlag(VehicleObjectFlags.AnyRoadType) && (model.Mode == TransportMode.Rail || model.Mode == TransportMode.Road))
		{
			bw.WriteS5Header(model.TrackType);
		}

		// track extras
		foreach (var x in model.RequiredTrackExtras)
		{
			bw.WriteS5Header(x);
		}

		// cargo types
		for (var i = 0; i < Constants.MaxCompatibleCargoCategories; ++i) // CompatibleCargoTypesLength should == CompatibleCargoCategories.Length
		{
			if (model.MaxCargo.Length < i || model.MaxCargo[i] == 0)
			{
				bw.WriteEmptyBytes(1); // write a 0 for MaxCargo - this indicates no more cargo and we skip the rest
				continue;
			}
			else
			{
				bw.Write(model.MaxCargo[i]);
			}

			var compatibleCargoCategories = model.CompatibleCargoCategories.Fill(Constants.MaxCompatibleCargoCategories, []).ToArray();
			foreach (var cc in compatibleCargoCategories[i])
			{
				bw.Write(BitConverter.GetBytes((uint16_t)cc));
				bw.Write(model.CargoTypeSpriteOffsets[cc]);
			}

			bw.Write(BitConverter.GetBytes((uint16_t)CargoCategory.NULL));
		}

		// animation
		bw.WriteS5HeaderList(model.AnimationHeaders);

		// compatible vehicles
		bw.WriteS5HeaderList(model.CompatibleVehicles);

		// rack rail
		if (model.Flags.HasFlag(VehicleObjectFlags.RackRail))
		{
			bw.WriteS5Header(model.RackRail);
		}

		// driving sound
		if (model.DrivingSoundType != DrivingSoundType.None)
		{
			bw.WriteS5Header(model.Sound);
		}

		// driving start sounds
		bw.WriteS5HeaderList(model.StartSounds);
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

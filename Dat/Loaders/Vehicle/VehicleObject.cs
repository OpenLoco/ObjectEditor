using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Vehicle;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class VehicleObjectLoader : IDatObjectLoader
{
	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream stream, LocoObject obj) => throw new NotImplementedException();
}

static class SimpleAnimationTypeConverter
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
public enum DatSimpleAnimationType : uint8_t
{
	None = 0,
	SteamPuff1,
	SteamPuff2,
	SteamPuff3,
	DieselExhaust1,
	ElectricSpark1,
	ElectricSpark2,
	DieselExhaust2,
	ShipWake
}

[LocoStructSize(0x15E)]
[LocoStructType(DatObjectType.Vehicle)]
internal record VehicleObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] DatTransportMode Mode,
	[property: LocoStructOffset(0x03)] DatVehicleType Type,
	[property: LocoStructOffset(0x04)] uint8_t NumCarComponents,
	[property: LocoStructOffset(0x05), LocoStructVariableLoad, Browsable(false)] object_id TrackTypeId,
	[property: LocoStructOffset(0x06)] uint8_t NumRequiredTrackExtras,
	[property: LocoStructOffset(0x07)] uint8_t CostIndex,
	[property: LocoStructOffset(0x08)] int16_t CostFactor,
	[property: LocoStructOffset(0x0A)] uint8_t Reliability,
	[property: LocoStructOffset(0x0B)] uint8_t RunCostIndex,
	[property: LocoStructOffset(0x0C)] int16_t RunCostFactor,
	[property: LocoStructOffset(0x0E)] DatCompanyColourType SpecialColourSchemeIndex, // this is an instance property and doesn't need to be saved in the definition as it can be calculated from other properties
	[property: LocoStructOffset(0x0F)] uint8_t NumCompatibleVehicles,
	[property: LocoStructOffset(0x10), LocoArrayLength(8), LocoStructVariableLoad] List<S5Header> CompatibleVehicles,
	[property: LocoStructOffset(0x20), LocoArrayLength(4), LocoStructVariableLoad] List<S5Header> RequiredTrackExtras,
	[property: LocoStructOffset(0x24), LocoArrayLength(4)] VehicleObjectCar[] CarComponents,
	[property: LocoStructOffset(0x3C), LocoArrayLength(VehicleObject.MaxBodySprites)] BodySprite[] BodySprites,
	[property: LocoStructOffset(0xB4), LocoArrayLength(VehicleObject.MaxBogieSprites)] BogieSprite[] BogieSprites,
	[property: LocoStructOffset(0xD8)] uint16_t Power,
	[property: LocoStructOffset(0xDA)] Speed16 Speed,
	[property: LocoStructOffset(0xDC)] Speed16 RackSpeed,
	[property: LocoStructOffset(0xDE)] uint16_t Weight,
	[property: LocoStructOffset(0xE0)] DatVehicleObjectFlags Flags,
	[property: LocoStructOffset(0xE2), LocoArrayLength(VehicleObject.CompatibleCargoTypesLength), LocoStructVariableLoad] List<uint8_t> MaxCargo,
	[property: LocoStructOffset(0xE4), LocoArrayLength(VehicleObject.CompatibleCargoTypesLength), LocoStructVariableLoad, Browsable(false)] List<List<DatCargoCategory>> CompatibleCargoCategories,
	[property: LocoStructOffset(0xEC), LocoArrayLength(VehicleObject.CargoTypeSpriteOffsetsLength), LocoStructVariableLoad] Dictionary<DatCargoCategory, uint8_t> CargoTypeSpriteOffsets,
	[property: LocoStructOffset(0x10C), LocoStructVariableLoad, Browsable(false)] uint8_t _NumSimultaneousCargoTypes,
	[property: LocoStructOffset(0x10D), LocoArrayLength(VehicleObject.AnimationCount)] SimpleAnimation[] Animation,
	[property: LocoStructOffset(0x113)] uint8_t ShipWakeOffset, // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
	[property: LocoStructOffset(0x114)] uint16_t DesignedYear,
	[property: LocoStructOffset(0x116)] uint16_t ObsoleteYear,
	[property: LocoStructOffset(0x118), LocoStructVariableLoad, Browsable(false)] object_id RackRailType,
	[property: LocoStructOffset(0x119)] DatDrivingSoundType DrivingSoundType,
	// this is a union...length is the length of the longest union struct, which is Engine2Sound. make the byte[] not visible in editor
	[property: LocoStructOffset(0x11A), LocoArrayLength(VehicleObject.MaxUnionSoundStructLength), Browsable(false)] byte[] SoundPropertiesData,
	//union
	//{
	//	VehicleObjectFrictionSound friction,
	//	VehicleObjectEngine1Sound engine1,
	//	VehicleObjectEngine2Sound engine2,
	//}
	//sound,
	[property: LocoStructOffset(0x135), LocoArrayLength(0x15A - 0x135), LocoStructVariableLoad] List<uint8_t> var_135,
	[property: LocoStructOffset(0x15A)] uint8_t NumStartSounds,
	[property: LocoStructOffset(0x15B), LocoArrayLength(3), LocoStructVariableLoad] List<S5Header> StartSounds
) : ILocoStruct, ILocoStructVariableData, ILocoStructPostLoad
{
	public const int MaxUnionSoundStructLength = 0x1B;
	public const int MaxBodySprites = 4;
	public const int MaxBogieSprites = 2;
	public const int AnimationCount = 2;
	public const int CompatibleCargoTypesLength = 2;
	public const int CargoTypeSpriteOffsetsLength = 32;

	public VehicleObject() : this(0, DatTransportMode.Rail, DatVehicleType.Train, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, [], [], [], [], [], 0, 0, 0, 0, DatVehicleObjectFlags.None, [], [[], []], [], 0, [], 0, 0, 0, 0, DatDrivingSoundType.None, [], [], 0, [])
	{ }

	public T GetSoundAs<T>() where T : ILocoStruct
		=> (T)ByteReader.ReadLocoStruct(SoundPropertiesData.AsSpan()[..ObjectAttributes.StructSize<T>()], typeof(T));

	public FrictionSound? FrictionSound
	{
		get => DrivingSoundType == DatDrivingSoundType.Friction ? GetSoundAs<FrictionSound>() : null;
		set
		{
			if (value != null)
			{
				ByteWriter.WriteLocoStruct(value).CopyTo(SoundPropertiesData);
			}
		}
	}

	public SimpleMotorSound? SimpleMotorSound
	{
		get => DrivingSoundType == DatDrivingSoundType.SimpleMotor ? GetSoundAs<SimpleMotorSound>() : null;
		set
		{
			if (value != null)
			{
				ByteWriter.WriteLocoStruct(value).CopyTo(SoundPropertiesData);
			}
		}
	}

	public GearboxMotorSound? GearboxMotorSound
	{
		get => DrivingSoundType == DatDrivingSoundType.GearboxMotor ? GetSoundAs<GearboxMotorSound>() : null;
		set
		{
			if (value != null)
			{
				ByteWriter.WriteLocoStruct(value).CopyTo(SoundPropertiesData);
			}
		}
	}

	// this hack is because winforms won't show a list of lists properly...
	public List<DatCargoCategory> CompatibleCargoCategories1 { get => CompatibleCargoCategories[0]; set => CompatibleCargoCategories[0] = value; }
	public List<DatCargoCategory> CompatibleCargoCategories2 { get => CompatibleCargoCategories[1]; set => CompatibleCargoCategories[1] = value; }

	public uint8_t NumSimultaneousCargoTypes { get; set; }

	public S5Header? TrackType { get; set; }
	public S5Header? RackRail { get; set; }
	public S5Header? Sound { get; set; }

	public List<S5Header> AnimationHeaders { get; set; } = [];

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// track type
		if (!Flags.HasFlag(DatVehicleObjectFlags.AnyRoadType) && (Mode == DatTransportMode.Rail || Mode == DatTransportMode.Road))
		{
			TrackType = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];
		}

		// track extras
		RequiredTrackExtras.Clear();
		RequiredTrackExtras.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumRequiredTrackExtras));
		remainingData = remainingData[(S5Header.StructLength * NumRequiredTrackExtras)..];

		// compatible cargo types
		CompatibleCargoCategories.Clear();
		for (var i = 0; i < CompatibleCargoTypesLength; ++i)
		{
			CompatibleCargoCategories.Add([]);
		}

		CargoTypeSpriteOffsets.Clear();
		MaxCargo.Clear();

		for (var i = 0; i < CompatibleCargoTypesLength; ++i)
		{
			var index = NumSimultaneousCargoTypes;
			MaxCargo.Add(remainingData[0]);
			remainingData = remainingData[1..]; // uint8_t

			if (MaxCargo[index] == 0)
			{
				continue;
			}

			while ((DatCargoCategory)BitConverter.ToUInt16(remainingData[0..2]) != DatCargoCategory.NULL)
			{
				var cargoCategory = (DatCargoCategory)BitConverter.ToUInt16(remainingData[0..2]);
				remainingData = remainingData[2..]; // uint16_t

				var cargoTypeSpriteOffset = remainingData[0];
				remainingData = remainingData[1..]; // uint8_t

				CompatibleCargoCategories[index].Add(cargoCategory);

				if (!CargoTypeSpriteOffsets.TryAdd(cargoCategory, cargoTypeSpriteOffset))
				{
					// invalid object - shouldn't have 2 cargo types that are the same
				}
			}

			remainingData = remainingData[2..]; // uint16_t, skips the 0xFFFF bytes

			if (CompatibleCargoCategories[index].Count == 0)
			{
				MaxCargo[index] = 0;
			}
			else
			{
				NumSimultaneousCargoTypes++;
			}
		}

		// animation
		AnimationHeaders.Clear();
		foreach (var anim in Animation)
		{
			if (anim.Type == DatSimpleAnimationType.None.Convert())
			{
				continue;
			}

			var animation = S5Header.Read(remainingData[..S5Header.StructLength]);
			AnimationHeaders.Add(animation);
			remainingData = remainingData[S5Header.StructLength..];
		}

		// numCompat
		CompatibleVehicles.Clear();
		CompatibleVehicles.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatibleVehicles));
		remainingData = remainingData[(S5Header.StructLength * NumCompatibleVehicles)..];

		// rack rail
		if (Flags.HasFlag(DatVehicleObjectFlags.RackRail))
		{
			RackRail = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];
		}

		// driving sound
		if (DrivingSoundType != DatDrivingSoundType.None)
		{
			Sound = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];
		}

		// driving/start sounds
		StartSounds.Clear();
		const int mask = 127;
		var count = NumStartSounds & mask;
		for (var i = 0; i < count; ++i)
		{
			var startSound = S5Header.Read(remainingData[..S5Header.StructLength]);
			StartSounds.Add(startSound);
			remainingData = remainingData[S5Header.StructLength..];
		}

		return remainingData;
	}

	public ReadOnlySpan<byte> SaveVariable()
	{
		var ms = new MemoryStream();

		// track type
		if (!Flags.HasFlag(DatVehicleObjectFlags.AnyRoadType) && (Mode == DatTransportMode.Rail || Mode == DatTransportMode.Road))
		{
			ms.Write(TrackType!.Write());
		}

		// track extras
		foreach (var x in RequiredTrackExtras)
		{
			ms.Write(x.Write());
		}

		// cargo types
		for (var i = 0; i < CompatibleCargoTypesLength; ++i) // CompatibleCargoTypesLength should == CompatibleCargoCategories.Length
		{
			if (MaxCargo.Count < i || MaxCargo[i] == 0)
			{
				ms.WriteByte(0); // write a 0 for MaxCargo - this indicates no more cargo and we skip the rest
				continue;
			}
			else
			{
				ms.WriteByte(MaxCargo[i]);
			}

			foreach (var cc in CompatibleCargoCategories[i])
			{
				ms.Write(BitConverter.GetBytes((uint16_t)cc));
				ms.WriteByte(CargoTypeSpriteOffsets[cc]);
			}

			ms.Write(BitConverter.GetBytes((uint16_t)DatCargoCategory.NULL));
		}

		// animation
		foreach (var x in AnimationHeaders)
		{
			ms.Write(x.Write());
		}

		// numCompat

		if (NumCompatibleVehicles != CompatibleVehicles.Count)
		{
			throw new ArgumentOutOfRangeException(nameof(NumCompatibleVehicles), $"NumCompatibleVehicles ({NumCompatibleVehicles}) must be equal to CompatibleVehicles.Count ({CompatibleVehicles.Count})");
		}

		foreach (var x in CompatibleVehicles)
		{
			ms.Write(x.Write());
		}

		// rack rail
		if (Flags.HasFlag(DatVehicleObjectFlags.RackRail))
		{
			ms.Write(RackRail!.Write());
		}

		// driving sound
		if (DrivingSoundType != DatDrivingSoundType.None)
		{
			ms.Write(Sound!.Write());
		}

		// driving start sounds
		foreach (var x in StartSounds)
		{
			ms.Write(x.Write());
		}

		return ms.ToArray();
	}

	public void PostLoad()
	{
		var offset = 0;

		// setup body sprites
		foreach (var bodySprite in BodySprites)
		{
			if (!bodySprite.Flags.HasFlag(DatBodySpriteFlags.HasSprites))
			{
				continue;
			}

			var initial = offset;
			bodySprite.FlatImageId = (uint)offset;
			var curr = offset;
			bodySprite.FlatYawAccuracy = GetYawAccuracyFlat(bodySprite.NumFlatRotationFrames);

			bodySprite.NumFramesPerRotation = (byte)((bodySprite.NumAnimationFrames * bodySprite.NumCargoFrames * bodySprite.NumRollFrames) + (bodySprite.Flags.HasFlag(DatBodySpriteFlags.HasBrakingLights) ? 1 : 0)); // be careful of overflow here...
			var numFlatFrames = (byte)(bodySprite.NumFramesPerRotation * bodySprite.NumFlatRotationFrames);
			offset += numFlatFrames / (bodySprite.Flags.HasFlag(DatBodySpriteFlags.RotationalSymmetry) ? 2 : 1);
			bodySprite.ImageIds[BodySpriteSlopeType.Flat] = [.. Enumerable.Range(curr, offset - curr)];

			if (bodySprite.Flags.HasFlag(DatBodySpriteFlags.HasGentleSprites))
			{
				bodySprite.GentleImageId = (uint)offset;
				curr = offset;
				var numGentleFrames = bodySprite.NumFramesPerRotation * 8;
				offset += numGentleFrames / (bodySprite.Flags.HasFlag(DatBodySpriteFlags.RotationalSymmetry) ? 2 : 1);
				bodySprite.ImageIds[BodySpriteSlopeType.Gentle] = [.. Enumerable.Range(curr, offset - curr)];

				bodySprite.SlopedImageId = (uint)offset;
				curr = offset;
				bodySprite.SlopedYawAccuracy = GetYawAccuracySloped(bodySprite.NumSlopedRotationFrames);
				var numSlopedFrames = bodySprite.NumFramesPerRotation * bodySprite.NumSlopedRotationFrames * 2;
				offset += numSlopedFrames / (bodySprite.Flags.HasFlag(DatBodySpriteFlags.RotationalSymmetry) ? 2 : 1);
				bodySprite.ImageIds[BodySpriteSlopeType.Sloped] = [.. Enumerable.Range(curr, offset - curr)];

				if (bodySprite.Flags.HasFlag(DatBodySpriteFlags.HasSteepSprites))
				{
					bodySprite.SteepImageId = (uint)offset;
					curr = offset;
					var numSteepFrames = bodySprite.NumFramesPerRotation * 8;
					offset += numSteepFrames / (bodySprite.Flags.HasFlag(DatBodySpriteFlags.RotationalSymmetry) ? 2 : 1);
					bodySprite.ImageIds[BodySpriteSlopeType.Steep] = [.. Enumerable.Range(curr, offset - curr)];

					// TODO: add these two together??
					bodySprite.UnkImageId1 = (uint)offset;
					curr = offset;
					var numUnkFrames = bodySprite.NumSlopedRotationFrames * bodySprite.NumFramesPerRotation * 2;
					offset += numUnkFrames / (bodySprite.Flags.HasFlag(DatBodySpriteFlags.RotationalSymmetry) ? 2 : 1);
					bodySprite.ImageIds[BodySpriteSlopeType.unk1] = [.. Enumerable.Range(curr, offset - curr)];
				}
			}

			if (bodySprite.Flags.HasFlag(DatBodySpriteFlags.HasUnkSprites))
			{
				//bodySprite.UnkImageId2 = offset;
				curr = offset;
				var numUnkFrames = bodySprite.NumFlatRotationFrames * 3;
				offset += numUnkFrames / (bodySprite.Flags.HasFlag(DatBodySpriteFlags.RotationalSymmetry) ? 2 : 1);
				bodySprite.ImageIds[BodySpriteSlopeType.unk2] = [.. Enumerable.Range(curr, offset)];
			}

			bodySprite.NumImages = offset - initial; // (int)(offset - bodySprite.FlatImageId);

			//if (bodySprite.FlatImageId + numImages <= ObjectManager::getTotalNumImages())
			//{
			//	var extents = Gfx::getImagesMaxExtent(ImageId(bodySprite.FlatImageId), numImages);
			//	bodySprite.Width = extents.width;
			//	bodySprite.HeightNegative = extents.heightNegative;
			//	bodySprite.HeightPositive = extents.heightPositive;
			//}
			//else
			//{
			//	// This is a bad object! But will keep loading
			//	Logging::error("Object has too few images for body sprites!");
			//	bodySprite.flatImageId = ImageId::kIndexUndefined;
			//	bodySprite.gentleImageId = ImageId::kIndexUndefined;
			//	bodySprite.steepImageId = ImageId::kIndexUndefined;
			//	bodySprite.unkImageId = ImageId::kIndexUndefined;
			//}
		}

		// setup bogie sprites
		foreach (var bogieSprite in BogieSprites)
		{
			if (!bogieSprite.Flags.HasFlag(DatBogieSpriteFlags.HasSprites))
			{
				continue;
			}

			bogieSprite.NumRollSprites = bogieSprite.RollStates;

			var initial = offset;
			var curr = offset;

			var numRollFrames = bogieSprite.NumRollSprites * 32;
			offset += numRollFrames / (bogieSprite.Flags.HasFlag(DatBogieSpriteFlags.RotationalSymmetry) ? 2 : 1);
			bogieSprite.ImageIds[BogieSpriteSlopeType.Flat] = [.. Enumerable.Range(curr, offset - curr)];

			if (bogieSprite.Flags.HasFlag(DatBogieSpriteFlags.HasGentleSprites))
			{
				//bogieSprite.GentleImageIds = offset;
				curr = offset;
				var numGentleFrames = bogieSprite.NumRollSprites * 64;
				offset += numGentleFrames / (bogieSprite.Flags.HasFlag(DatBogieSpriteFlags.RotationalSymmetry) ? 2 : 1);
				bogieSprite.ImageIds[BogieSpriteSlopeType.Gentle] = [.. Enumerable.Range(curr, offset - curr)];

				if (bogieSprite.Flags.HasFlag(DatBogieSpriteFlags.HasSteepSprites))
				{
					//bogieSprite.SteepImageIds = offset;
					curr = offset;
					var numSteepFrames = bogieSprite.NumRollSprites * 64;
					offset += numSteepFrames / (bogieSprite.Flags.HasFlag(DatBogieSpriteFlags.RotationalSymmetry) ? 2 : 1);
					bogieSprite.ImageIds[BogieSpriteSlopeType.Steep] = [.. Enumerable.Range(curr, offset - curr)];
				}
			}

			bogieSprite.NumImages = offset - initial;
			//if (bogieSprite.flatImageIds + numImages <= ObjectManager::getTotalNumImages())
			//{
			//	var extents = Gfx::getImagesMaxExtent(ImageId(bogieSprite.flatImageIds), numImages);
			//	bogieSprite.width = extents.width;
			//	bogieSprite.heightNegative = extents.heightNegative;
			//	bogieSprite.heightPositive = extents.heightPositive;
			//}
			//else
			//{
			//	// This is a bad object! But we will keep loading anyway!
			//	Logging::error("Object has too few images for bogie sprites!");
			//	bogieSprite.flatImageIds = ImageId::kIndexUndefined;
			//	bogieSprite.gentleImageIds = ImageId::kIndexUndefined;
			//	bogieSprite.steepImageIds = ImageId::kIndexUndefined;
			//}
		}
	}

	static uint8_t GetYawAccuracyFlat(uint8_t numFrames)
		=> numFrames switch
		{
			8 => 1,
			16 => 2,
			32 => 3,
			_ => 4,
		};

	static uint8_t GetYawAccuracySloped(uint8_t numFrames)
		=> numFrames switch
		{
			4 => 0,
			8 => 1,
			16 => 2,
			_ => 3,
		};

	public bool Validate()
	{
		if (CostIndex > 32)
		{
			return false;
		}

		if (RunCostIndex > 32)
		{
			return false;
		}

		if (CostFactor <= 0)
		{
			return false;
		}

		if (RunCostFactor < 0)
		{
			return false;
		}

		if (Flags.HasFlag(DatVehicleObjectFlags.AnyRoadType))
		{
			if (NumRequiredTrackExtras != 0)
			{
				return false;
			}

			if (Flags.HasFlag(DatVehicleObjectFlags.RackRail))
			{
				return false;
			}
		}

		if (NumRequiredTrackExtras > 4)
		{
			return false;
		}

		if (NumSimultaneousCargoTypes > 2)
		{
			return false;
		}

		if (NumCompatibleVehicles > 8)
		{
			return false;
		}

		if (RackSpeed > Speed)
		{
			return false;
		}

		foreach (var bodySprite in BodySprites)
		{
			if (!bodySprite.Flags.HasFlag(DatBodySpriteFlags.HasSprites))
			{
				continue;
			}

			switch (bodySprite.NumFlatRotationFrames)
			{
				case 8:
				case 16:
				case 32:
				case 64:
				case 128:
					break;
				default:
					return false;
			}

			switch (bodySprite.NumSlopedRotationFrames)
			{
				case 4:
				case 8:
				case 16:
				case 32:
					break;
				default:
					return false;
			}

			switch (bodySprite.NumAnimationFrames)
			{
				case 1:
				case 2:
				case 4:
					break;
				default:
					return false;
			}

			if (bodySprite.NumCargoLoadFrames is < 1 or > 5)
			{
				return false;
			}

			switch (bodySprite.NumRollFrames)
			{
				case 1:
				case 3:
					break;
				default:
					return false;
			}
		}

		foreach (var bogieSprite in BogieSprites)
		{
			if (!bogieSprite.Flags.HasFlag(DatBogieSpriteFlags.HasSprites))
			{
				continue;
			}

			switch (bogieSprite.RollStates)
			{
				case 1:
				case 2:
				case 4:
					break;
				default:
					return false;
			}
		}

		return true;
	}
}

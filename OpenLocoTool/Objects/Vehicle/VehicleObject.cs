using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualBasic;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x15E)]
	[LocoStructType(ObjectType.Vehicle)]
	[LocoStringTable("Name")]
	public record VehicleObject(
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name ,
		[property: LocoStructOffset(0x02)] TransportMode Mode,
		[property: LocoStructOffset(0x03)] VehicleType Type,
		[property: LocoStructOffset(0x04)] uint8_t var_04,
		//[LocoStructOffset(0x05)] object_id TrackType ,
		[property: LocoStructOffset(0x06)] uint8_t NumMods,
		[property: LocoStructOffset(0x07)] uint8_t CostIndex,
		[property: LocoStructOffset(0x08)] int16_t CostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t Reliability,
		[property: LocoStructOffset(0x0B)] uint8_t RunCostIndex,
		[property: LocoStructOffset(0x0C)] int16_t RunCostFactor,
		[property: LocoStructOffset(0x0E)] uint8_t ColourType,
		[property: LocoStructOffset(0x0F)] uint8_t NumCompat,
		[property: LocoStructOffset(0x10), LocoArrayLength(8), LocoStructVariableLoad] List<S5Header> CompatibleVehicles,
		[property: LocoStructOffset(0x20), LocoArrayLength(4), LocoStructVariableLoad] List<S5Header> RequiredTrackExtras,
		[property: LocoStructOffset(0x24), LocoArrayLength(4)] VehicleObjectUnk[] var_24,
		[property: LocoStructOffset(0x3C), LocoArrayLength(VehicleObject.MaxBodySprites), LocoStructVariableLoad] List<BodySprite> BodySprites,
		[property: LocoStructOffset(0xB4), LocoArrayLength(2), LocoStructVariableLoad] List<BogieSprite> BogieSprites,
		[property: LocoStructOffset(0xD8)] uint16_t Power,
		[property: LocoStructOffset(0xDA)] Speed16 Speed,
		[property: LocoStructOffset(0xDC)] Speed16 RackSpeed,
		[property: LocoStructOffset(0xDE)] uint16_t Weight,
		[property: LocoStructOffset(0xE0)] VehicleObjectFlags Flags,
		[property: LocoStructOffset(0xE2), LocoArrayLength(2), LocoStructVariableLoad] List<uint8_t> MaxCargo,
		[property: LocoStructOffset(0xE4), LocoArrayLength(VehicleObject.CargoTypesLength), LocoStructVariableLoad] List<uint32_t> CargoTypes,
		[property: LocoStructOffset(0xEC), LocoArrayLength(VehicleObject.CargoTypeSpriteOffsetsLength), LocoStructVariableLoad] List<uint8_t> CargoTypeSpriteOffsets,
		//[property: LocoStructOffset(0x10C), LocoStructVariableLoad] uint8_t NumSimultaneousCargoTypes,
		[property: LocoStructOffset(0x10D), LocoArrayLength(VehicleObject.AnimationCount)] SimpleAnimation[] Animation,
		[property: LocoStructOffset(0x113)] uint8_t var_113,
		[property: LocoStructOffset(0x114)] uint16_t Designed,
		[property: LocoStructOffset(0x116)] uint16_t Obsolete,
		[property: LocoStructOffset(0x118), LocoStructVariableLoad] uint8_t RackRailType,
		[property: LocoStructOffset(0x119)] DrivingSoundType DrivingSoundType,
		//union
		//{
		//	VehicleObjectFrictionSound friction,
		//	VehicleObjectEngine1Sound engine1,
		//	VehicleObjectEngine2Sound engine2,
		//}
		//sound,
		[property: LocoStructOffset(0x135), LocoArrayLength(0x15A - 0x135), LocoStructVariableLoad] List<uint8_t> pad_135,
		[property: LocoStructOffset(0x15A)] uint8_t NumStartSounds,
		[property: LocoStructOffset(0x15B), LocoArrayLength(3), LocoStructVariableLoad] List<S5Header> StartSounds
	) : ILocoStruct, ILocoStructVariableData
	{
		public const int MaxBodySprites = 4;
		public const int AnimationCount = 2;
		public const int CargoTypesLength = 2;
		public const int CargoTypeSpriteOffsetsLength = 32;
		public List<uint16_t> CargoMatchFlags = [];

		public List<CargoObject> CompatibleCargo = [];

		public uint8_t NumSimultaneousCargoTypes { get; set; }

		public S5Header? TrackType { get; set; }
		public S5Header? RackRail { get; set; }
		public S5Header? DrivingSound { get; set; }

		public List<S5Header> AnimationHeaders { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// track type
			if (!Flags.HasFlag(VehicleObjectFlags.unk_09) && (Mode == TransportMode.Rail || Mode == TransportMode.Road))
			{
				TrackType = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			// track extra
			RequiredTrackExtras.Clear();
			RequiredTrackExtras.AddRange(SawyerStreamReader.LoadVariableHeaders(remainingData, NumMods));
			remainingData = remainingData[(S5Header.StructLength * NumMods)..];

			// cargo types
			// this whole bullshit is mostly copied and pasted from openloco
			// but we need to do it to a) load the cargo match flags and b) to move the stream to the right offset to load the next variable data
			// afterwards, we'll do nice c# load of the cargo based on the match flags
			MaxCargo.Clear();
			CargoMatchFlags.Clear();

			CargoTypes.Clear();
			CargoTypes.AddRange(Enumerable.Repeat(0U, CargoTypesLength));

			CargoTypeSpriteOffsets.Clear();
			CargoTypeSpriteOffsets.AddRange(Enumerable.Repeat((byte)0, CargoTypeSpriteOffsetsLength));

			for (var i = 0; i < CargoTypesLength; ++i)
			{
				var index = NumSimultaneousCargoTypes;
				MaxCargo.Add(remainingData[0]);
				remainingData = remainingData[1..]; // uint8_t

				if (MaxCargo[index] == 0)
				{
					continue;
				}

				var ptr = BitConverter.ToUInt16(remainingData[0..2]);
				while (ptr != 0xFFFF)
				{
					var cargoMatchFlags = BitConverter.ToUInt16(remainingData[0..2]);
					CargoMatchFlags.Add(cargoMatchFlags);
					remainingData = remainingData[2..]; // uint16_t

					var unk = remainingData[0];
					remainingData = remainingData[1..]; // uint8_t

					var cargoObjs = SObjectManager.Get<CargoObject>(ObjectType.Cargo);

					for (var cargoType = 0; cargoType < 32; ++cargoType) // 32 is ObjectType::MaxObjects[cargo]
					{
						// until the rest of this is implemented, these values will be wrong
						// but as long as they're non-zero to pass the == 0 check below, it'll work
						CargoTypes[index] |= 1U << cargoType;
						CargoTypeSpriteOffsets[cargoType] = unk;
					}

					ptr = BitConverter.ToUInt16(remainingData[0..2]);
				}

				remainingData = remainingData[2..]; // uint16_t

				if (CargoTypes[index] == 0)
				{
					MaxCargo[index] = 0;
				}
				else
				{
					NumSimultaneousCargoTypes++;
				}
			}

			foreach (var cargo in SObjectManager.Get<CargoObject>(ObjectType.Cargo))
			{
				if (CargoMatchFlags.Contains(cargo.MatchFlags))
				{
					CompatibleCargo.Add(cargo);
				}
			}

			// animation
			AnimationHeaders.Clear();
			foreach (var anim in Animation)
			{
				if (anim.Type == SimpleAnimationType.None)
				{
					continue;
				}

				var animation = S5Header.Read(remainingData[..S5Header.StructLength]);
				AnimationHeaders.Add(animation);
				remainingData = remainingData[S5Header.StructLength..];
			}

			// numCompat
			CompatibleVehicles.Clear();
			CompatibleVehicles.AddRange(SawyerStreamReader.LoadVariableHeaders(remainingData, NumCompat));
			remainingData = remainingData[(S5Header.StructLength * NumCompat)..];

			// rack rail
			if (Flags.HasFlag(VehicleObjectFlags.RackRail))
			{
				RackRail = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			// driving sound
			if (DrivingSoundType != DrivingSoundType.None)
			{
				DrivingSound = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			// driving/start sound
			StartSounds.Clear();
			var mask = 127;
			for (var i = 0; i < (NumStartSounds & mask); ++i)
			{
				var startSound = S5Header.Read(remainingData[..S5Header.StructLength]);
				StartSounds.Add(startSound);
				remainingData = remainingData[S5Header.StructLength..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			throw new NotImplementedException();
		}
	}
}

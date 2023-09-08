using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x15E)]
	public class VehicleObject : ILocoStruct, ILocoStructVariableData
	{
		public const ObjectType ObjType = ObjectType.Vehicle;
		public const int StructSize = 0x15E;
		public const int MaxBodySprites = 4;

		public VehicleObject(ushort name, TransportMode mode, VehicleType type, byte var_04, byte trackType, byte numMods, byte costIndex, short costFactor, byte reliability, byte runCostIndex, short runCostFactor, byte colourType, byte numCompat, ushort[] compatibleVehicles, byte[] requiredTrackExtras, VehicleObjectUnk[] var_24, BodySprite[] bodySprites, BogieSprite[] bogieSprites, ushort power, short speed, short rackSpeed, ushort weight, VehicleObjectFlags flags, byte[] maxCargo, uint[] cargoTypes, byte[] cargoTypeSpriteOffsets, byte numSimultaneousCargoTypes, SimpleAnimation[] animation, byte var_113, ushort designed, ushort obsolete, byte rackRailType, DrivingSoundType drivingSoundType, byte[] pad_135, byte numStartSounds, byte[] startSounds)
		{
			Name = name;
			Mode = mode;
			Type = type;
			this.var_04 = var_04;
			TrackType = trackType;
			NumMods = numMods;
			CostIndex = costIndex;
			CostFactor = costFactor;
			Reliability = reliability;
			RunCostIndex = runCostIndex;
			RunCostFactor = runCostFactor;
			ColourType = colourType;
			NumCompat = numCompat;
			CompatibleVehicles = compatibleVehicles;
			RequiredTrackExtras = requiredTrackExtras;
			this.var_24 = var_24;
			BodySprites = bodySprites;
			BogieSprites = bogieSprites;
			Power = power;
			Speed = speed;
			RackSpeed = rackSpeed;
			Weight = weight;
			Flags = flags;
			MaxCargo = maxCargo;
			CargoTypes = cargoTypes;
			CargoTypeSpriteOffsets = cargoTypeSpriteOffsets;
			NumSimultaneousCargoTypes = numSimultaneousCargoTypes;
			Animation = animation;
			this.var_113 = var_113;
			Designed = designed;
			Obsolete = obsolete;
			RackRailType = rackRailType;
			DrivingSoundType = drivingSoundType;
			this.pad_135 = pad_135;
			NumStartSounds = numStartSounds;
			StartSounds = startSounds;
		}

		[LocoStructOffset(0x00)] public string_id Name { get; set; }
		[LocoStructOffset(0x02)] public TransportMode Mode { get; set; }
		[LocoStructOffset(0x03)] public VehicleType Type { get; set; }
		[LocoStructOffset(0x04)] public uint8_t var_04 { get; set; }
		[LocoStructOffset(0x05)] public uint8_t TrackType { get; set; }
		[LocoStructOffset(0x06)] public uint8_t NumMods { get; set; }
		[LocoStructOffset(0x07)] public uint8_t CostIndex { get; set; }
		[LocoStructOffset(0x08)] public int16_t CostFactor { get; set; }
		[LocoStructOffset(0x0A)] public uint8_t Reliability { get; set; }
		[LocoStructOffset(0x0B)] public uint8_t RunCostIndex { get; set; }
		[LocoStructOffset(0x0C)] public int16_t RunCostFactor { get; set; }
		[LocoStructOffset(0x0E)] public uint8_t ColourType { get; set; }
		[LocoStructOffset(0x0F)] public uint8_t NumCompat { get; set; }
		[LocoStructOffset(0x10), LocoArrayLength(8)] public uint16_t[] CompatibleVehicles { get; set; } // array of compatible vehicle_types
		[LocoStructOffset(0x20), LocoArrayLength(4)] public uint8_t[] RequiredTrackExtras { get; set; }
		[LocoStructOffset(0x24), LocoArrayLength(4)] public VehicleObjectUnk[] var_24 { get; set; }
		[LocoStructOffset(0x3C), LocoArrayLength(MaxBodySprites)] public BodySprite[] BodySprites { get; set; }
		[LocoStructOffset(0xB4), LocoArrayLength(2)] public BogieSprite[] BogieSprites { get; set; }
		[LocoStructOffset(0xD8)] public uint16_t Power { get; set; }
		[LocoStructOffset(0xDA)] public Speed16 Speed { get; set; }
		[LocoStructOffset(0xDC)] public Speed16 RackSpeed { get; set; }
		[LocoStructOffset(0xDE)] public uint16_t Weight { get; set; }
		[LocoStructOffset(0xE0)] public VehicleObjectFlags Flags { get; set; }
		[LocoStructOffset(0xE2), LocoArrayLength(2)] public uint8_t[] MaxCargo { get; set; } // size is relative to the first cargoTypes
		[LocoStructOffset(0xE4), LocoArrayLength(2)] public uint32_t[] CargoTypes { get; set; }
		[LocoStructOffset(0xEC), LocoArrayLength(32)] public uint8_t[] CargoTypeSpriteOffsets { get; set; }
		[LocoStructOffset(0x10C)] public uint8_t NumSimultaneousCargoTypes { get; set; }
		[LocoStructOffset(0x10D), LocoArrayLength(2)] public SimpleAnimation[] Animation { get; set; }
		[LocoStructOffset(0x113)] public uint8_t var_113 { get; set; }
		[LocoStructOffset(0x114)] public uint16_t Designed { get; set; }
		[LocoStructOffset(0x116)] public uint16_t Obsolete { get; set; }
		[LocoStructOffset(0x118)] public uint8_t RackRailType { get; set; }
		[LocoStructOffset(0x119)] public DrivingSoundType DrivingSoundType { get; set; }
		//union
		//{
		//	VehicleObjectFrictionSound friction,
		//	VehicleObjectEngine1Sound engine1,
		//	VehicleObjectEngine2Sound engine2,
		//}
		//sound,
		[LocoStructOffset(0x135), LocoArrayLength(0x15A - 0x135)] public uint8_t[] pad_135 { get; set; }
		[LocoStructOffset(0x15A)] public uint8_t NumStartSounds { get; set; } // use mask when accessing kHasCrossingWhistle stuffed in (1 << 7)
		[LocoStructOffset(0x15B), LocoArrayLength(3)] public SoundObjectId[] StartSounds { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			var dependentObjects = new List<S5Header>();

			const byte trackType = 0xFF;

			// dependent objects
			if (!Flags.HasFlag(VehicleObjectFlags.unk_09) && (Mode == TransportMode.Rail || Mode == TransportMode.Road))
			{
				var trackHeader = S5Header.Read(remainingData);
				dependentObjects.Add(trackHeader);
				TrackType = trackType;
				// load the object handle for the track header, and set tracktype to its id

				remainingData = remainingData[S5Header.StructLength..];
			}

			// track mods
			remainingData = remainingData[(S5Header.StructLength * NumMods)..];

			// cargo types
			for (var i = 0; i < CargoTypes.Length; ++i)
			{
				var index = NumSimultaneousCargoTypes;
				MaxCargo[i] = remainingData[0];
				remainingData = remainingData[1..]; // uint8_t

				if (MaxCargo[index] == 0)
				{
					continue;
				}

				var ptr = BitConverter.ToUInt16(remainingData[0..2]);
				while (ptr != (ushort)0xFFFF)
				{
					var cargoMatchFlags = BitConverter.ToUInt16(remainingData[0..2]);
					remainingData = remainingData[2..]; // uint16_t

					var unk = remainingData[0];
					remainingData = remainingData[1..]; // uint8_t

					for (var cargoType = 0; cargoType < 32; ++cargoType) // 32 is ObjectType::MaxObjects[cargo]
					{
						// until the rest of this is implemented, these values will be wrong
						// but as long as they're non-zero to pass the == 0 check below, it'll work
						CargoTypes[index] |= (1U << cargoType);
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

			// animation
			foreach (var anim in Animation)
			{
				if (anim.Type == SimpleAnimationType.None)
				{
					continue;
				}

				remainingData = remainingData[S5Header.StructLength..];
			}

			// numCompat
			remainingData = remainingData[(S5Header.StructLength * NumCompat)..];

			// rack rail
			if (Flags.HasFlag(VehicleObjectFlags.RackRail))
			{
				remainingData = remainingData[S5Header.StructLength..];
			}

			// driving sound
			if (DrivingSoundType != DrivingSoundType.None)
			{
				remainingData = remainingData[S5Header.StructLength..];
			}

			var mask = 127;
			// driving sound
			remainingData = remainingData[(S5Header.StructLength * (NumStartSounds & mask))..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}

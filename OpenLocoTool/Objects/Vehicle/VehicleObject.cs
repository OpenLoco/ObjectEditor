using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x15E)]
	[LocoStructType(ObjectType.Vehicle)]
	[LocoStringTable("Name")]
	public record VehicleObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] TransportMode Mode,
		[property: LocoStructOffset(0x03)] VehicleType Type,
		[property: LocoStructOffset(0x04)] uint8_t var_04,
		[property: LocoStructOffset(0x05), LocoStructVariableLoad, Browsable(false)] object_id TrackTypeId,
		[property: LocoStructOffset(0x06)] uint8_t NumTrackExtras,
		[property: LocoStructOffset(0x07)] uint8_t CostIndex,
		[property: LocoStructOffset(0x08)] int16_t CostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t Reliability,
		[property: LocoStructOffset(0x0B)] uint8_t RunCostIndex,
		[property: LocoStructOffset(0x0C)] int16_t RunCostFactor,
		[property: LocoStructOffset(0x0E)] uint8_t ColourType,
		[property: LocoStructOffset(0x0F)] uint8_t NumCompatibleVehicles,
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
		[property: LocoStructOffset(0xE4), LocoArrayLength(VehicleObject.CompatibleCargoTypesLength), LocoStructVariableLoad, Browsable(false)] List<List<CargoCategory>> CompatibleCargoCategories,
		[property: LocoStructOffset(0xEC), LocoArrayLength(VehicleObject.CargoTypeSpriteOffsetsLength), LocoStructVariableLoad] Dictionary<CargoCategory, uint8_t> CargoTypeSpriteOffsets,
		[property: LocoStructOffset(0x10C), LocoStructVariableLoad, Browsable(false)] uint8_t _NumSimultaneousCargoTypes,
		[property: LocoStructOffset(0x10D), LocoArrayLength(VehicleObject.AnimationCount)] SimpleAnimation[] Animation,
		[property: LocoStructOffset(0x113)] uint8_t var_113,
		[property: LocoStructOffset(0x114)] uint16_t Designed,
		[property: LocoStructOffset(0x116)] uint16_t Obsolete,
		[property: LocoStructOffset(0x118), LocoStructVariableLoad] uint8_t RackRailType,
		[property: LocoStructOffset(0x119)] DrivingSoundType SoundType,
		// this is a union...length is the length of the longest union struct, which is Engine2Sound. make the byte[] not visible in editor
		[property: LocoStructOffset(0x11A), LocoArrayLength(VehicleObject.MaxUnionSoundStructLength), Browsable(false)] byte[] SoundPropertiesData,
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
		public const int MaxUnionSoundStructLength = 0x1B;
		public const int MaxBodySprites = 4;
		public const int AnimationCount = 2;
		public const int CompatibleCargoTypesLength = 2;
		public const int CargoTypeSpriteOffsetsLength = 32;

		public FrictionSound? SoundPropertyFriction
		{
			get
			{
				return SoundType == DrivingSoundType.Friction
					? (FrictionSound)ByteReader.ReadLocoStruct(SoundPropertiesData.AsSpan()[..ObjectAttributes.StructSize<FrictionSound>()], typeof(FrictionSound))
					: null;
			}
			set
			{
				if (value != null)
				{
					ByteWriter.WriteLocoStruct(value).CopyTo(SoundPropertiesData);
				}
				else
				{
					for (var i = 0; i < MaxUnionSoundStructLength; ++i)
					{
						SoundPropertiesData[i] = 0;
					}
				}
			}
		}

		public Engine1Sound? SoundPropertyEngine1
		{
			get
			{
				return SoundType == DrivingSoundType.Engine1
					? (Engine1Sound)ByteReader.ReadLocoStruct(SoundPropertiesData.AsSpan()[..ObjectAttributes.StructSize<Engine1Sound>()], typeof(Engine1Sound))
					: null;
			}
			set
			{
				if (value != null)
				{
					ByteWriter.WriteLocoStruct(value).CopyTo(SoundPropertiesData);
				}
				else
				{
					for (var i = 0; i < MaxUnionSoundStructLength; ++i)
					{
						SoundPropertiesData[i] = 0;
					}
				}
			}
		}

		public Engine2Sound? SoundPropertyEngine2
		{
			get
			{
				return SoundType == DrivingSoundType.Engine2
					? (Engine2Sound)ByteReader.ReadLocoStruct(SoundPropertiesData.AsSpan()[..ObjectAttributes.StructSize<Engine2Sound>()], typeof(Engine2Sound))
					: null;
			}
			set
			{
				if (value != null)
				{
					ByteWriter.WriteLocoStruct(value).CopyTo(SoundPropertiesData);
				}
				else
				{
					for (var i = 0; i < MaxUnionSoundStructLength; ++i)
					{
						SoundPropertiesData[i] = 0;
					}
				}
			}
		}

		// this hack is because winforms won't show a list of lists properly...
		public List<CargoCategory> CompatibleCargoCategories1 { get => CompatibleCargoCategories[0]; set => CompatibleCargoCategories[0] = value; }
		public List<CargoCategory> CompatibleCargoCategories2 { get => CompatibleCargoCategories[1]; set => CompatibleCargoCategories[1] = value; }

		public uint8_t NumSimultaneousCargoTypes { get; set; }

		public S5Header? TrackType { get; set; }
		public S5Header? RackRail { get; set; }
		public S5Header? Sound { get; set; }

		public List<S5Header> AnimationHeaders { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// track type
			if (!Flags.HasFlag(VehicleObjectFlags.unk_09) && (Mode == TransportMode.Rail || Mode == TransportMode.Road))
			{
				TrackType = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			// track extras
			RequiredTrackExtras.Clear();
			RequiredTrackExtras.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumTrackExtras));
			remainingData = remainingData[(S5Header.StructLength * NumTrackExtras)..];

			// compatible cargo types
			CompatibleCargoCategories.Clear();
			for (var i = 0; i < CompatibleCargoTypesLength; ++i)
			{
				CompatibleCargoCategories.Add([]);
			}

			var cargoCategories = SObjectManager.Get<CargoObject>(ObjectType.Cargo)
				.Select(c => c.CargoCategory)
				.Distinct()
				.OrderBy(cc => (uint16_t)cc);

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

				var ptr = BitConverter.ToUInt16(remainingData[0..2]);
				while (ptr != (uint16_t)CargoCategory.NULL)
				{
					var vehicleCargoCategory = (CargoCategory)BitConverter.ToUInt16(remainingData[0..2]);
					remainingData = remainingData[2..]; // uint16_t

					var cargoTypeSpriteOffset = remainingData[0];
					remainingData = remainingData[1..]; // uint8_t

					CompatibleCargoCategories[index].Add(vehicleCargoCategory);

					if (!CargoTypeSpriteOffsets.TryAdd(vehicleCargoCategory, cargoTypeSpriteOffset))
					{
						// invalid object - shouldn't have 2 cargo types that are the same
					}

					// advance ptr
					ptr = BitConverter.ToUInt16(remainingData[0..2]);
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
			CompatibleVehicles.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatibleVehicles));
			remainingData = remainingData[(S5Header.StructLength * NumCompatibleVehicles)..];

			// rack rail
			if (Flags.HasFlag(VehicleObjectFlags.RackRail))
			{
				RackRail = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			// driving sound
			if (SoundType != DrivingSoundType.None)
			{
				Sound = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			// driving/start sounds
			StartSounds.Clear();
			var mask = 127;
			var count = NumStartSounds & mask;
			for (var i = 0; i < count; ++i)
			{
				var startSound = S5Header.Read(remainingData[..S5Header.StructLength]);
				StartSounds.Add(startSound);
				remainingData = remainingData[S5Header.StructLength..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			var ms = new MemoryStream();

			// track type
			if (!Flags.HasFlag(VehicleObjectFlags.unk_09) && (Mode == TransportMode.Rail || Mode == TransportMode.Road))
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
				ms.WriteByte(MaxCargo[i]);

				if (MaxCargo[i] == 0)
				{
					continue;
				}

				foreach (var cc in CompatibleCargoCategories[i])
				{
					ms.Write(BitConverter.GetBytes((uint16_t)cc));
					ms.WriteByte(CargoTypeSpriteOffsets[cc]);
				}

				ms.WriteByte(0xFF);
				ms.WriteByte(0xFF);
			}

			// animation
			foreach (var x in CompatibleVehicles)
			{
				ms.Write(x.Write());
			}

			// numCompat
			foreach (var x in AnimationHeaders)
			{
				ms.Write(x.Write());
			}

			// rack rail
			if (Flags.HasFlag(VehicleObjectFlags.RackRail))
			{
				ms.Write(RackRail!.Write());
			}

			// driving sound
			if (SoundType != DrivingSoundType.None)
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
	}
}

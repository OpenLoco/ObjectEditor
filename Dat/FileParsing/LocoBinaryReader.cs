using Dat.Converters;
using Dat.Types;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using System.Text;

namespace Dat.FileParsing;

public class LocoBinaryReader : BinaryReader
{
	public LocoBinaryReader(Stream input) : base(input, Encoding.UTF8, leaveOpen: true)
	{ }

	[Obsolete("This method does not correctly read bytes from the stream. Use PeekByte instead.")]
	public override int PeekChar()
		=> throw new NotImplementedException("This method does not correctly read bytes from the stream. Use PeekByte instead.");

	[Obsolete("Use as specific Read method for the data type you want to read.")]
	public override int Read()
		=> throw new NotImplementedException("Use as specific Read method for the data type you want to read.");

	public void SkipTerminator()
		=> SkipByte(1);

	public void SkipByte(int count = 1)
		=> _ = BaseStream.Seek(count, SeekOrigin.Current);

	public void SkipUInt16(int count = 1)
		=> _ = BaseStream.Seek(2 * count, SeekOrigin.Current);

	public void SkipInt16(int count = 1)
		=> _ = BaseStream.Seek(2 * count, SeekOrigin.Current);

	public void SkipUInt32(int count = 1)
		=> _ = BaseStream.Seek(4 * count, SeekOrigin.Current);

	public void SkipInt32(int count = 1)
		=> _ = BaseStream.Seek(4 * count, SeekOrigin.Current);

	public void SkipStringId(int count = 1)
		=> _ = BaseStream.Seek(2 * count, SeekOrigin.Current);

	public void SkipObjectId(int count = 1)
		=> _ = BaseStream.Seek(1 * count, SeekOrigin.Current);

	public void SkipImageId(int count = 1)
		=> _ = BaseStream.Seek(4 * count, SeekOrigin.Current);

	public void SkipPointer(int count = 1)
		=> _ = BaseStream.Seek(4 * count, SeekOrigin.Current);

	public byte PeekByte(int ahead = 0)
	{
		var currentPosition = BaseStream.Position;
		_ = BaseStream.Seek(ahead, SeekOrigin.Current);
		var value = ReadByte();
		_ = BaseStream.Seek(currentPosition, SeekOrigin.Begin);
		return value;
	}

	public uint16_t PeekUInt16(int ahead = 0)
	{
		var currentPosition = BaseStream.Position;
		_ = BaseStream.Seek(ahead, SeekOrigin.Current);
		var value = ReadUInt16();
		_ = BaseStream.Seek(currentPosition, SeekOrigin.Begin);
		return value;
	}

	public byte[] ReadToEnd()
	{
		var currentPosition = BaseStream.Position;
		var length = (int)(BaseStream.Length - currentPosition);
		var buffer = new byte[length];
		_ = Read(buffer, 0, length);
		return buffer;
	}

	public static List<ObjectModelHeader> ReadS5HeaderList(Stream stream, int count)
	{
		using var br = new LocoBinaryReader(stream);
		return br.ReadS5HeaderList(count);
	}

	public List<ObjectModelHeader> ReadS5HeaderList(int count)
	{
		List<ObjectModelHeader> result = [];
		for (var i = 0; i < count; ++i)
		{
			var header = ReadS5Header();
			if (header != null)
			{
				result.Add(header);
			}
		}

		return result;
	}

	public List<ObjectModelHeader> ReadS5HeaderList()
	{
		List<ObjectModelHeader> result = [];

		while (PeekByte() != LocoConstants.Terminator)
		{
			var header = ReadS5Header();
			if (header != null)
			{
				result.Add(header);
			}
		}
		SkipTerminator();
		return result;
	}

	public ObjectModelHeader? ReadS5Header()
	{
		var header = S5Header.Read(ReadBytes(S5Header.StructLength));
		// vanilla objects will have sourcegameflag == 0 and checksum == 0. custom objects will have a checksum specified - may need custom handling
		return header.Checksum != 0 || header.Flags != 255
			? new ObjectModelHeader(header.Name, header.Checksum, header.ObjectType.Convert(), header.ObjectSource.Convert())
			: default;
	}

	public List<uint8_t> ReadBuildingHeights(int count)
		=> [.. ReadBytes(count)];

	public BuildingPartAnimation ReadBuildingPartAnimation()
		=> new(ReadByte(), ReadByte());

	public List<BuildingPartAnimation> ReadBuildingAnimations(int count)
	{
		List<BuildingPartAnimation> result = [];

		for (var i = 0; i < count; ++i)
		{
			result.Add(ReadBuildingPartAnimation());
		}

		return result;
	}

	public List<List<byte>> ReadBuildingVariations(int count)
	{
		var result = new List<List<byte>>();

		for (var i = 0; i < count; ++i)
		{
			List<byte> tmp = [];
			byte b;
			while ((b = ReadByte()) != LocoConstants.Terminator)
			{
				tmp.Add(b);
			}

			result.Add(tmp);
		}

		return result;
	}

	public SoundEffectWaveFormat ReadSoundEffect()
		=> new()
		{
			WaveFormatTag = ReadInt16(),
			Channels = ReadInt16(),
			SampleRate = ReadInt32(),
			AverageBytesPerSecond = ReadInt32(),
			BlockAlign = ReadInt16(),
			BitsPerSample = ReadInt16(),
			ExtraSize = ReadInt16()
		};

	public VehicleObjectCar[] ReadCarComponents(int count)
	{
		var result = new VehicleObjectCar[count];

		for (var i = 0; i < count; ++i)
		{
			result[i] = new VehicleObjectCar
			{
				FrontBogiePosition = ReadByte(),
				BackBogiePosition = ReadByte(),
				FrontBogieSpriteIndex = ReadByte(),
				BackBogieSpriteIndex = ReadByte(),
				BodySpriteIndex = ReadByte(),
				var_05 = ReadByte(),
			};
		}

		return result;
	}

	public BodySprite[] ReadBodySprites(int count)
	{
		var result = new BodySprite[count];

		for (var i = 0; i < count; ++i)
		{
			result[i] = new BodySprite
			{
				NumFlatRotationFrames = ReadByte(),
				NumSlopedRotationFrames = ReadByte(),
				NumAnimationFrames = ReadByte(),
				NumCargoLoadFrames = ReadByte(),
				NumCargoFrames = ReadByte(),
				NumRollFrames = ReadByte(),
				HalfLength = ReadByte(),
				Flags = (BodySpriteFlags)ReadByte(),
				_Width = ReadByte(),
				_HeightNegative = ReadByte(),
				_HeightPositive = ReadByte(),
				_FlatYawAccuracy = ReadByte(),
				_SlopedYawAccuracy = ReadByte(),
				_NumFramesPerRotation = ReadByte(),
				_FlatImageId = 0,
				_UnkImageId = 0,
				_GentleImageId = 0,
				_SteepImageId = 0,
			};

			SkipImageId();
			SkipImageId();
			SkipImageId();
			SkipImageId();
		}

		return result;
	}

	public BogieSprite[] ReadBogieSprites(int count)
	{
		var result = new BogieSprite[count];

		for (var i = 0; i < count; ++i)
		{
			result[i] = new BogieSprite
			{
				RollStates = ReadByte(),
				Flags = (BogieSpriteFlags)ReadByte(),
				Width = ReadByte(),
				HeightNegative = ReadByte(),
				HeightPositive = ReadByte(),
				_NumRollSprites = ReadByte(),
				_FlatImageIds = 0,
				_GentleImageIds = 0,
				_SteepImageIds = 0,
			};
			SkipImageId();
			SkipImageId();
			SkipImageId();
		}

		return result;
	}

	public SimpleAnimation[] ReadSimpleAnimations(int count)
	{
		var result = new SimpleAnimation[count];

		for (var i = 0; i < count; ++i)
		{
			result[i] = new SimpleAnimation
			{
				ObjectId = ReadByte(),
				Height = ReadByte(),
				Type = (SimpleAnimationType)ReadByte(),
			};
		}

		return result;
	}

	public FrictionSound ReadFrictionSound()
		=> new()
		{
			SoundObjectId = ReadByte(),
			MinSpeed = ReadInt32(),
			SpeedFreqFactor = ReadByte(),
			BaseFrequency = ReadUInt16(),
			SpeedVolumeFactor = ReadByte(),
			BaseVolume = ReadByte(),
			MaxVolume = ReadByte(),
		};

	public SimpleMotorSound ReadSimpleMotorSound()
		=> new()
		{
			SoundObjectId = ReadByte(),
			IdleFrequency = ReadUInt16(),
			IdleVolume = ReadByte(),
			CoastingFrequency = ReadUInt16(),
			CoastingVolume = ReadByte(),
			AccelerationBaseFrequency = ReadUInt16(),
			AccelerationVolume = ReadByte(),
			FreqIncreaseStep = ReadUInt16(),
			FreqDecreaseStep = ReadUInt16(),
			VolumeIncreaseStep = ReadByte(),
			VolumeDecreaseStep = ReadByte(),
			SpeedFrequencyFactor = ReadByte(),
		};

	public GearboxMotorSound ReadGearboxMotorSound()
		=> new()
		{
			SoundObjectId = ReadByte(),
			IdleFrequency = ReadUInt16(),
			IdleVolume = ReadByte(),
			FirstGearFrequency = ReadUInt16(),
			FirstGearSpeed = ReadInt16(),
			SecondGearFrequencyOffset = ReadUInt16(),
			SecondGearSpeed = ReadInt16(),
			ThirdGearFrequencyOffset = ReadUInt16(),
			ThirdGearSpeed = ReadInt16(),
			FourthGearFrequencyOffset = ReadUInt16(),
			CoastingVolume = ReadByte(),
			AcceleratingVolume = ReadByte(),
			FreqIncreaseStep = ReadUInt16(),
			FreqDecreaseStep = ReadUInt16(),
			VolumeIncreaseStep = ReadByte(),
			VolumeDecreaseStep = ReadByte(),
			SpeedFrequencyFactor = ReadByte(),
		};
}

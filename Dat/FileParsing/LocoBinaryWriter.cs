using Common;
using Dat.Converters;
using Dat.Types;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using System.Text;

namespace Dat.FileParsing;

public class LocoBinaryWriter : BinaryWriter
{
	public LocoBinaryWriter(Stream output) : base(output, Encoding.UTF8, leaveOpen: true)
	{ }

	public void WriteTerminator()
		=> base.Write(LocoConstants.Terminator);

	public void WriteEmptyBytes(int count)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((uint8_t)0);
		}
	}

	public void WriteEmptyStringId(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((string_id)0);
		}
	}

	public void WriteEmptyImageId(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((image_id)0);
		}
	}

	public void WriteEmptyObjectId(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((object_id)0);
		}
	}

	public void WriteEmptyPointer(int count = 1)
	{
		for (var i = 0; i < count; i++)
		{
			base.Write((uint32_t)0);
		}
	}

	public void WriteS5Header(S5Header header)
		=> Write(header.Write());

	public void WriteS5Header(ObjectModelHeader? header)
		=> WriteS5Header(header?.Convert() ?? S5Header.NullHeader);

	public void WriteS5HeaderList(IEnumerable<ObjectModelHeader> headers, int minimum = 0)
	{
		foreach (var header in headers
			.Select(x => x.Convert())
			.Fill(minimum, S5Header.NullHeader))
		{
			WriteS5Header(header);
		}
	}

	public void Write(List<uint8_t> heights)
		=> base.Write([.. heights]);

	public void Write(List<BuildingPartAnimation> animations)
	{
		foreach (var x in animations)
		{
			Write(x.NumFrames);
			Write(x.AnimationSpeed);
		}
	}

	public void Write(List<List<uint8_t>> variations)
	{
		foreach (var x in variations)
		{
			base.Write([.. x]);
			WriteTerminator();
		}
	}

	public void Write(SoundEffectWaveFormat sfx)
	{
		Write(sfx.WaveFormatTag);
		Write(sfx.Channels);
		Write(sfx.SampleRate);
		Write(sfx.AverageBytesPerSecond);
		Write(sfx.BlockAlign);
		Write(sfx.BitsPerSample);
		Write(sfx.ExtraSize);
	}

	public void Write(VehicleObjectCar[] cars)
	{
		foreach (var car in cars)
		{
			Write(car.FrontBogiePosition);
			Write(car.BackBogiePosition);
			Write(car.FrontBogieSpriteIndex);
			Write(car.BackBogieSpriteIndex);
			Write(car.BodySpriteIndex);
			Write(car.var_05);
		}
	}

	public void Write(BodySprite[] bodies)
	{
		foreach (var body in bodies)
		{
			Write(body.NumFlatRotationFrames);
			Write(body.NumSlopedRotationFrames);
			Write(body.NumAnimationFrames);
			Write(body.NumCargoLoadFrames);
			Write(body.NumCargoFrames);
			Write(body.NumRollFrames);
			Write(body.HalfLength);
			Write((uint8_t)body.Flags);
			Write(body._Width);
			Write(body._HeightNegative);
			Write(body._HeightPositive);
			Write(body._FlatYawAccuracy);
			Write(body._SlopedYawAccuracy);
			Write(body._NumFramesPerRotation);
			WriteEmptyImageId(4); // image ids not part of object definition
		}
	}

	public void Write(BogieSprite[] bogies)
	{
		foreach (var bogie in bogies)
		{
			Write(bogie.RollStates);
			Write((uint8_t)bogie.Flags);
			Write(bogie.Width);
			Write(bogie.HeightNegative);
			Write(bogie.HeightPositive);
			Write(bogie._NumRollSprites);
			WriteEmptyImageId(3); // image ids not part of object definition
		}
	}

	public void Write(SimpleAnimation[] animations)
	{
		foreach (var animation in animations)
		{
			Write(animation.ObjectId);
			Write(animation.Height);
			Write((uint8_t)animation.Type);
		}
	}

	public void Write(FrictionSound sound)
	{
		Write(sound.SoundObjectId);
		Write(sound.MinSpeed);
		Write(sound.SpeedFreqFactor);
		Write(sound.BaseFrequency);
		Write(sound.SpeedVolumeFactor);
		Write(sound.BaseVolume);
		Write(sound.MaxVolume);
	}

	public void Write(SimpleMotorSound sound)
	{
		Write(sound.SoundObjectId);
		Write(sound.IdleFrequency);
		Write(sound.IdleVolume);
		Write(sound.CoastingFrequency);
		Write(sound.CoastingVolume);
		Write(sound.AccelerationBaseFrequency);
		Write(sound.AccelerationVolume);
		Write(sound.FreqIncreaseStep);
		Write(sound.FreqDecreaseStep);
		Write(sound.VolumeIncreaseStep);
		Write(sound.VolumeDecreaseStep);
		Write(sound.SpeedFrequencyFactor);
	}

	public void Write(GearboxMotorSound sound)
	{
		Write(sound.SoundObjectId);
		Write(sound.IdleFrequency);
		Write(sound.IdleVolume);
		Write(sound.FirstGearFrequency);
		Write(sound.FirstGearSpeed);
		Write(sound.SecondGearFrequencyOffset);
		Write(sound.SecondGearSpeed);
		Write(sound.ThirdGearFrequencyOffset);
		Write(sound.ThirdGearSpeed);
		Write(sound.FourthGearFrequencyOffset);
		Write(sound.CoastingVolume);
		Write(sound.AcceleratingVolume);
		Write(sound.FreqIncreaseStep);
		Write(sound.FreqDecreaseStep);
		Write(sound.VolumeIncreaseStep);
		Write(sound.VolumeDecreaseStep);
		Write(sound.SpeedFrequencyFactor);
	}
}

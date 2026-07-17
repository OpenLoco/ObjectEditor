using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class GearboxMotorSoundViewModel : ReactiveObject
{
	private readonly GearboxMotorSound _model;

	public GearboxMotorSoundViewModel(GearboxMotorSound model)
	{
		_model = model;
	}

	public ObjectModelHeader SoundObject
	{
		get => _model.SoundObject;
		set
		{
			_model.SoundObject = value;
			this.RaisePropertyChanged(nameof(SoundObject));
		}
	}

	public uint16_t IdleFrequency
	{
		get => _model.IdleFrequency;
		set
		{
			_model.IdleFrequency = value;
			this.RaisePropertyChanged(nameof(IdleFrequency));
		}
	}

	public uint8_t IdleVolume
	{
		get => _model.IdleVolume;
		set
		{
			_model.IdleVolume = value;
			this.RaisePropertyChanged(nameof(IdleVolume));
		}
	}

	public uint16_t FirstGearFrequency
	{
		get => _model.FirstGearFrequency;
		set
		{
			_model.FirstGearFrequency = value;
			this.RaisePropertyChanged(nameof(FirstGearFrequency));
		}
	}

	public int16_t FirstGearSpeed
	{
		get => _model.FirstGearSpeed;
		set
		{
			_model.FirstGearSpeed = value;
			this.RaisePropertyChanged(nameof(FirstGearSpeed));
		}
	}

	public uint16_t SecondGearFrequencyOffset
	{
		get => _model.SecondGearFrequencyOffset;
		set
		{
			_model.SecondGearFrequencyOffset = value;
			this.RaisePropertyChanged(nameof(SecondGearFrequencyOffset));
		}
	}

	public int16_t SecondGearSpeed
	{
		get => _model.SecondGearSpeed;
		set
		{
			_model.SecondGearSpeed = value;
			this.RaisePropertyChanged(nameof(SecondGearSpeed));
		}
	}

	public uint16_t ThirdGearFrequencyOffset
	{
		get => _model.ThirdGearFrequencyOffset;
		set
		{
			_model.ThirdGearFrequencyOffset = value;
			this.RaisePropertyChanged(nameof(ThirdGearFrequencyOffset));
		}
	}

	public int16_t ThirdGearSpeed
	{
		get => _model.ThirdGearSpeed;
		set
		{
			_model.ThirdGearSpeed = value;
			this.RaisePropertyChanged(nameof(ThirdGearSpeed));
		}
	}

	public uint16_t FourthGearFrequencyOffset
	{
		get => _model.FourthGearFrequencyOffset;
		set
		{
			_model.FourthGearFrequencyOffset = value;
			this.RaisePropertyChanged(nameof(FourthGearFrequencyOffset));
		}
	}

	public uint8_t CoastingVolume
	{
		get => _model.CoastingVolume;
		set
		{
			_model.CoastingVolume = value;
			this.RaisePropertyChanged(nameof(CoastingVolume));
		}
	}

	public uint8_t AcceleratingVolume
	{
		get => _model.AcceleratingVolume;
		set
		{
			_model.AcceleratingVolume = value;
			this.RaisePropertyChanged(nameof(AcceleratingVolume));
		}
	}

	public uint16_t FreqIncreaseStep
	{
		get => _model.FreqIncreaseStep;
		set
		{
			_model.FreqIncreaseStep = value;
			this.RaisePropertyChanged(nameof(FreqIncreaseStep));
		}
	}

	public uint16_t FreqDecreaseStep
	{
		get => _model.FreqDecreaseStep;
		set
		{
			_model.FreqDecreaseStep = value;
			this.RaisePropertyChanged(nameof(FreqDecreaseStep));
		}
	}

	public uint8_t VolumeIncreaseStep
	{
		get => _model.VolumeIncreaseStep;
		set
		{
			_model.VolumeIncreaseStep = value;
			this.RaisePropertyChanged(nameof(VolumeIncreaseStep));
		}
	}

	public uint8_t VolumeDecreaseStep
	{
		get => _model.VolumeDecreaseStep;
		set
		{
			_model.VolumeDecreaseStep = value;
			this.RaisePropertyChanged(nameof(VolumeDecreaseStep));
		}
	}

	public uint8_t SpeedFrequencyFactor
	{
		get => _model.SpeedFrequencyFactor;
		set
		{
			_model.SpeedFrequencyFactor = value;
			this.RaisePropertyChanged(nameof(SpeedFrequencyFactor));
		}
	}
}
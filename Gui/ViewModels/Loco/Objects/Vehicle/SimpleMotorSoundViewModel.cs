using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class SimpleMotorSoundViewModel : ReactiveObject
{
	private readonly SimpleMotorSound _model;

	public SimpleMotorSoundViewModel(SimpleMotorSound model)
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

	public uint16_t CoastingFrequency
	{
		get => _model.CoastingFrequency;
		set
		{
			_model.CoastingFrequency = value;
			this.RaisePropertyChanged(nameof(CoastingFrequency));
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

	public uint16_t AccelerationBaseFrequency
	{
		get => _model.AccelerationBaseFrequency;
		set
		{
			_model.AccelerationBaseFrequency = value;
			this.RaisePropertyChanged(nameof(AccelerationBaseFrequency));
		}
	}

	public uint8_t AccelerationVolume
	{
		get => _model.AccelerationVolume;
		set
		{
			_model.AccelerationVolume = value;
			this.RaisePropertyChanged(nameof(AccelerationVolume));
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
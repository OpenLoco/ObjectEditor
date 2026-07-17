using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class FrictionSoundViewModel : ReactiveObject
{
	private readonly FrictionSound _model;

	public FrictionSoundViewModel(FrictionSound model)
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

	public int32_t MinSpeed
	{
		get => _model.MinSpeed;
		set
		{
			_model.MinSpeed = value;
			this.RaisePropertyChanged(nameof(MinSpeed));
		}
	}

	public uint8_t SpeedFreqFactor
	{
		get => _model.SpeedFreqFactor;
		set
		{
			_model.SpeedFreqFactor = value;
			this.RaisePropertyChanged(nameof(SpeedFreqFactor));
		}
	}

	public uint16_t BaseFrequency
	{
		get => _model.BaseFrequency;
		set
		{
			_model.BaseFrequency = value;
			this.RaisePropertyChanged(nameof(BaseFrequency));
		}
	}

	public uint8_t SpeedVolumeFactor
	{
		get => _model.SpeedVolumeFactor;
		set
		{
			_model.SpeedVolumeFactor = value;
			this.RaisePropertyChanged(nameof(SpeedVolumeFactor));
		}
	}

	public uint8_t BaseVolume
	{
		get => _model.BaseVolume;
		set
		{
			_model.BaseVolume = value;
			this.RaisePropertyChanged(nameof(BaseVolume));
		}
	}

	public uint8_t MaxVolume
	{
		get => _model.MaxVolume;
		set
		{
			_model.MaxVolume = value;
			this.RaisePropertyChanged(nameof(MaxVolume));
		}
	}
}
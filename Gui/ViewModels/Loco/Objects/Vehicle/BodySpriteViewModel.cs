using Definitions.ObjectModels.Objects.Vehicle;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BodySpriteViewModel : ReactiveObject
{
	private readonly BodySprite _model;

	public BodySpriteViewModel(BodySprite model)
	{
		_model = model;
	}

	public uint8_t NumFlatRotationFrames
	{
		get => _model.NumFlatRotationFrames;
		set
		{
			_model.NumFlatRotationFrames = value;
			this.RaisePropertyChanged(nameof(NumFlatRotationFrames));
		}
	}

	public uint8_t NumSlopedRotationFrames
	{
		get => _model.NumSlopedRotationFrames;
		set
		{
			_model.NumSlopedRotationFrames = value;
			this.RaisePropertyChanged(nameof(NumSlopedRotationFrames));
		}
	}

	public uint8_t NumAnimationFrames
	{
		get => _model.NumAnimationFrames;
		set
		{
			_model.NumAnimationFrames = value;
			this.RaisePropertyChanged(nameof(NumAnimationFrames));
		}
	}

	public uint8_t NumCargoLoadFrames
	{
		get => _model.NumCargoLoadFrames;
		set
		{
			_model.NumCargoLoadFrames = value;
			this.RaisePropertyChanged(nameof(NumCargoLoadFrames));
		}
	}

	public uint8_t NumCargoFrames
	{
		get => _model.NumCargoFrames;
		set
		{
			_model.NumCargoFrames = value;
			this.RaisePropertyChanged(nameof(NumCargoFrames));
		}
	}

	public uint8_t NumRollFrames
	{
		get => _model.NumRollFrames;
		set
		{
			_model.NumRollFrames = value;
			this.RaisePropertyChanged(nameof(NumRollFrames));
		}
	}

	public uint8_t HalfLength
	{
		get => _model.HalfLength;
		set
		{
			_model.HalfLength = value;
			this.RaisePropertyChanged(nameof(HalfLength));
		}
	}

	[EnumProhibitValues<BodySpriteFlags>(BodySpriteFlags.None)]
	public BodySpriteFlags Flags
	{
		get => _model.Flags;
		set
		{
			_model.Flags = value;
			this.RaisePropertyChanged(nameof(Flags));
		}
	}
}

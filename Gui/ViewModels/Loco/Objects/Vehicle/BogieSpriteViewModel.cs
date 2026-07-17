using Definitions.ObjectModels.Objects.Vehicle;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BogieSpriteViewModel : ReactiveObject
{
	private readonly BogieSprite _model;

	public BogieSpriteViewModel(BogieSprite model)
	{
		_model = model;
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

	public BogieSpriteFlags Flags
	{
		get => _model.Flags;
		set
		{
			_model.Flags = value;
			this.RaisePropertyChanged(nameof(Flags));
		}
	}

	public uint8_t Width
	{
		get => _model.Width;
		set
		{
			_model.Width = value;
			this.RaisePropertyChanged(nameof(Width));
		}
	}

	public uint8_t HeightNegative
	{
		get => _model.HeightNegative;
		set
		{
			_model.HeightNegative = value;
			this.RaisePropertyChanged(nameof(HeightNegative));
		}
	}

	public uint8_t HeightPositive
	{
		get => _model.HeightPositive;
		set
		{
			_model.HeightPositive = value;
			this.RaisePropertyChanged(nameof(HeightPositive));
		}
	}
}
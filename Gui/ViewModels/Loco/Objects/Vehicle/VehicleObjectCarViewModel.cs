using Definitions.ObjectModels.Objects.Vehicle;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class VehicleObjectCarViewModel : ReactiveObject
{
	private readonly VehicleObjectCar _model;

	public VehicleObjectCarViewModel(VehicleObjectCar model)
	{
		_model = model;
	}

	public uint8_t FrontBogiePosition
	{
		get => _model.FrontBogiePosition;
		set
		{
			_model.FrontBogiePosition = value;
			this.RaisePropertyChanged(nameof(FrontBogiePosition));
		}
	}

	public uint8_t BackBogiePosition
	{
		get => _model.BackBogiePosition;
		set
		{
			_model.BackBogiePosition = value;
			this.RaisePropertyChanged(nameof(BackBogiePosition));
		}
	}

	public uint8_t FrontBogieSpriteIndex
	{
		get => _model.FrontBogieSpriteIndex;
		set
		{
			_model.FrontBogieSpriteIndex = value;
			this.RaisePropertyChanged(nameof(FrontBogieSpriteIndex));
		}
	}

	public uint8_t BackBogieSpriteIndex
	{
		get => _model.BackBogieSpriteIndex;
		set
		{
			_model.BackBogieSpriteIndex = value;
			this.RaisePropertyChanged(nameof(BackBogieSpriteIndex));
		}
	}

	public uint8_t BodySpriteIndex
	{
		get => _model.BodySpriteIndex;
		set
		{
			_model.BodySpriteIndex = value;
			this.RaisePropertyChanged(nameof(BodySpriteIndex));
		}
	}

	public uint8_t EmitterHorizontalOffset
	{
		get => _model.EmitterHorizontalOffset;
		set
		{
			_model.EmitterHorizontalOffset = value;
			this.RaisePropertyChanged(nameof(EmitterHorizontalOffset));
		}
	}
}
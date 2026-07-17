using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class EmitterAnimationViewModel : ReactiveObject
{
	private readonly EmitterAnimation _model;

	public EmitterAnimationViewModel(EmitterAnimation model)
	{
		_model = model;
	}

	public EmitterAnimation Model => _model;

	public ObjectModelHeader? SteamObject
	{
		get => _model.SteamObject;
		set
		{
			_model.SteamObject = value;
			this.RaisePropertyChanged(nameof(SteamObject));
		}
	}

	public uint8_t EmitterVerticalPos
	{
		get => _model.EmitterVerticalPos;
		set
		{
			_model.EmitterVerticalPos = value;
			this.RaisePropertyChanged(nameof(EmitterVerticalPos));
		}
	}

	public SimpleAnimationType Type
	{
		get => _model.Type;
		set
		{
			_model.Type = value;
			this.RaisePropertyChanged(nameof(Type));
		}
	}
}

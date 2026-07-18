using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class EmitterAnimationViewModel : ReactiveObject
{
	private readonly EmitterAnimation _model;
	private readonly ObjectModelHeaderViewModel _steamObjectWrapper;

	public EmitterAnimationViewModel(EmitterAnimation model)
	{
		_model = model;
		_steamObjectWrapper = new ObjectModelHeaderViewModel(model.SteamObject ?? new ObjectModelHeader());
	}

	public ObjectModelHeaderViewModel SteamObject
	{
		get
		{
			// Keep the wrapper in sync with the model
			if (_model.SteamObject != null && _steamObjectWrapper.Model != _model.SteamObject)
			{
				_steamObjectWrapper.Model.Name = _model.SteamObject.Name;
				_steamObjectWrapper.Model.ObjectSource = _model.SteamObject.ObjectSource;
				_steamObjectWrapper.Model.ObjectType = _model.SteamObject.ObjectType;
			}
			else
			{
				_steamObjectWrapper.Model.Name = string.Empty;
				_steamObjectWrapper.Model.ObjectSource = ObjectSource.Custom;
				_steamObjectWrapper.Model.ObjectType = ObjectType.Steam;
			}
			return _steamObjectWrapper;
		}
		set
		{
			_model.SteamObject = value.Model;
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

using ReactiveUI;
using Definitions.ObjectModels;

namespace Gui.ViewModels;

public abstract class LocoObjectViewModel<T> : ReactiveObject, IObjectViewModel<ILocoStruct> where T : class, ILocoStruct
{
	public abstract T GetAsModel();

	ILocoStruct IObjectViewModel<ILocoStruct>.GetAsModel() => GetAsModel();
}

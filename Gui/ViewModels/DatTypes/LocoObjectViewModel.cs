using ReactiveUI;
using Definitions.ObjectModels;

namespace Gui.ViewModels;

public abstract class LocoObjectViewModel<T> : ReactiveObject, IObjectViewModel<ILocoStruct> where T : class, ILocoStruct
{
	public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct)
		=> GetAsStruct();

	public abstract T GetAsStruct();
}

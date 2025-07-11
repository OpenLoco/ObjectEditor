using Dat.Types;
using ReactiveUI;

namespace Gui.ViewModels
{
	public abstract class LocoObjectViewModel<T> : ReactiveObject, IObjectViewModel<ILocoStruct> where T : class, ILocoStruct
	{
		public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct)
			=> GetAsStruct((locoStruct as T)!);

		public abstract T GetAsStruct(T input);
	}
}

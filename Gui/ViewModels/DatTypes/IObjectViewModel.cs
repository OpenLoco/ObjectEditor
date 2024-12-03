using OpenLoco.Dat.Types;
using ReactiveUI;

namespace OpenLoco.Gui.ViewModels
{
	public interface IObjectViewModel<T>
	{
		T GetAsUnderlyingType(T underlyingType);
	}

	public abstract class LocoObjectViewModel<T> : ReactiveObject, IObjectViewModel<ILocoStruct> where T : class, ILocoStruct
	{
		public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct)
			=> GetAsStruct((locoStruct as T)!);

		public abstract T GetAsStruct(T input);
	}
}

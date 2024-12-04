namespace OpenLoco.Gui.ViewModels
{
	public interface IObjectViewModel<T>
	{
		T GetAsUnderlyingType(T underlyingType);
	}
}

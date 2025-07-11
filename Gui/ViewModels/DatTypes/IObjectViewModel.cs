namespace Gui.ViewModels
{
	// this is purely to bind to the UI elements since Avalonia XAML doesn't support binding to generic types
	public interface IObjectViewModel;

	public interface IObjectViewModel<T> : IObjectViewModel
	{
		T GetAsUnderlyingType(T underlyingType);
	}
}

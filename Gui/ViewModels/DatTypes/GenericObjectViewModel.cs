using Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace Gui.ViewModels
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GenericObjectViewModel : ReactiveObject, IObjectViewModel<ILocoStruct>
	{
		[Reactive]
		public required ILocoStruct Object { get; set; }

		public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct)
			=> locoStruct;
	}
}

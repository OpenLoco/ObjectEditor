using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace OpenLoco.Gui.ViewModels
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GenericObjectViewModel : ReactiveObject, IObjectViewModel
	{
		[Reactive] public required ILocoStruct Object { get; set; }

		public ILocoStruct GetAsLocoStruct(ILocoStruct locoStruct)
			=> locoStruct;
	}
}

using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenLoco.Gui.ViewModels
{
	public class TownNamesViewModel : ReactiveObject, IObjectViewModel
	{
		[Reactive, Length(6, 6), Editable(false)] public BindingList<Category> Categories { get; set; }

		public TownNamesViewModel(TownNamesObject tno) => Categories = new(tno.Categories);

		public ILocoStruct GetAsLocoStruct(ILocoStruct locoStruct)
			=> GetAsStruct((locoStruct as TownNamesObject)!);

		public TownNamesObject GetAsStruct(TownNamesObject baseTownNames)
			=> baseTownNames;
	}
}

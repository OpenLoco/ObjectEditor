using OpenLoco.Dat.Types;

namespace OpenLoco.Gui.ViewModels
{
	public interface IObjectViewModel
	{
		ILocoStruct GetAsLocoStruct(ILocoStruct locoStruct);
	}
}

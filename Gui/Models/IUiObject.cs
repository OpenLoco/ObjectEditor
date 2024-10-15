using System.ComponentModel;

namespace OpenLoco.Gui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IUiObject;
}

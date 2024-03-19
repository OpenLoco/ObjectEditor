using System.ComponentModel;

namespace OpenLoco.ObjectEditor.AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IUiObject { }
}

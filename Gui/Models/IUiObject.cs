using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IUiObject;
}

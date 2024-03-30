using OpenLoco.ObjectEditor.Headers;
using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(TypeListConverter))]
	public interface IUiObjectWithGraphics
	{
		public BindingList<G1Element32> G1Elements { get; set; }
	}
}

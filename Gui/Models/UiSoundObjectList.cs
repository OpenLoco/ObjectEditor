using System.Collections.Generic;
using System.ComponentModel;

namespace OpenLoco.Gui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiSoundObjectList : IUiObject
	{
		public string FileName { get; set; }
		public List<UiSoundObject> Audio { get; } = [];
	}
}

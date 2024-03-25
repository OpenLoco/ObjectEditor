using OpenLoco.ObjectEditor.DatFileParsing;
using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiLocoObject : IUiObject //, IUiObjectWithGraphics
	{
		public DatFileInfo DatFileInfo { get; set; }
		public ILocoObject LocoObject { get; set; }
	}
}

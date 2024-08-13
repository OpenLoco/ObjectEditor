using OpenLoco.ObjectEditor.DatFileParsing;
using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiLocoFile : IUiObject
	{
		public required DatFileInfo DatFileInfo { get; set; }
		public ILocoObject? LocoObject { get; set; }
	}
}

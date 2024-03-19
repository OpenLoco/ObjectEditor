using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenLoco.ObjectEditor.AvaGui.Models
{

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiLocoObject : IUiObject, IUiObjectWithGraphics
	{
		public DatFileInfo DatFileInfo { get; set; }
		public ILocoObject? LocoObject { get; set; }
		public List<G1Element32> G1Elements
		{
			get => LocoObject?.G1Elements;
			set => LocoObject.G1Elements = value;
		}
	}
}

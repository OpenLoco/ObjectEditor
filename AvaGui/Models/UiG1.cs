using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using System.Collections.Generic;
using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiG1 : IUiObject, IUiObjectWithGraphics
	{
		public G1Dat G1 { get; set; }

		public List<G1Element32> G1Elements
		{
			get => G1.G1Elements;
			set
			{
				G1.G1Elements.Clear();
				foreach (var x in value)
				{
					G1.G1Elements.Add(x);
				}

			}
		}
	}
}

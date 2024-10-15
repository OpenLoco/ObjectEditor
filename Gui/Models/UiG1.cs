using OpenLoco.Dat.Types;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenLoco.Gui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiG1 : IUiObject
	{
		public G1Dat G1 { get; set; }

		public List<G1Element32> G1Elements
		{
			get => G1.G1Elements;
			set
			{
				G1.G1Elements.Clear();
				G1.G1Elements.AddRange(value);
			}
		}
	}
}

using Dat.Types;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gui.Models;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class UiG1 : IUiObject
{
	public required G1Dat G1 { get; set; }

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

using Dat.Types;
using Definitions.ObjectModels.Types;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gui.Models;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class UiG1 : IUiObject
{
	public required G1Dat G1 { get; set; }

	public List<GraphicsElement> G1Elements
	{
		get => G1.GraphicsElements;
		set
		{
			G1.GraphicsElements.Clear();
			G1.GraphicsElements.AddRange(value);
		}
	}
}

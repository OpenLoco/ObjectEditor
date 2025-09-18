using Dat.Types;
using System.ComponentModel;

namespace Gui.Models;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class UiG1 : IUiObject
{
	public required G1Dat G1 { get; set; }
}

using Dat.Types;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Gui.Models;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class LocoUIObjectModel
{
	public LocoObject? LocoObject { get; set; }
	public LocoObjectMetadata? Metadata { get; set; }
	public DatInfo? DatInfo { get; set; }
}

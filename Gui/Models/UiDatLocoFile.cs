using Dat.Types;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Gui.Models;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class UiDatLocoFile
{
	public required DatFileInfo DatFileInfo { get; set; }
	public LocoObject? LocoObject { get; set; }
	public MetadataModel? Metadata { get; set; }
}

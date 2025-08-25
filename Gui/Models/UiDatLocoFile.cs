using Dat.Types;
using Definitions.ObjectModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gui.Models;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class UiDatLocoFile
{
	public required DatFileInfo DatFileInfo { get; set; }
	public LocoObject? LocoObject { get; set; }
	public IList<Image<Rgba32>> Images { get; set; } = [];
	public MetadataModel? Metadata { get; set; }
}

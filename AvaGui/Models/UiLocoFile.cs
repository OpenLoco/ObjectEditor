using Avalonia.Media.Imaging;
using OpenLoco.Dat.Types;
using System.Collections.Generic;
using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiLocoFile : IUiObject
	{
		public required DatFileInfo DatFileInfo { get; set; }
		public ILocoObject? LocoObject { get; set; }
		public IList<Bitmap> Images { get; set; } = [];
		public MetadataModel? Metadata { get; set; }
	}
}

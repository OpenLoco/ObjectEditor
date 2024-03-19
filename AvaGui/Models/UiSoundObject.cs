using OpenLoco.ObjectEditor.Types;
using System.ComponentModel;

namespace OpenLoco.ObjectEditor.AvaGui.Models
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiSoundObject
	{
		public string SoundName { get; set; }
		public RiffWavHeader Header { get; set; }
		public byte[] Data { get; set; }
	}
}

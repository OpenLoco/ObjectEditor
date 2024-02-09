using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Types;
using System.ComponentModel;

namespace OpenLocoObjectEditorGui
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IUiObject { }

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiLocoObject : IUiObject
	{
		public DatFileInfo DatFileInfo { get; set; }
		public ILocoObject? LocoObject { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiSoundObject
	{
		public string SoundName { get; set; }
		public RiffWavHeader Header { get; set; }
		public byte[] Data { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiSoundObjectList : IUiObject
	{
		public string FileName { get; set; }
		public List<UiSoundObject> Audio { get; set; } = [];
	}
}

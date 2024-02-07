using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Types;

namespace OpenLocoObjectEditorGui
{
	public interface IUiObject { }

	public class UiLocoObject : IUiObject
	{
		public DatFileInfo DatFileInfo { get; set; }
		public ILocoObject LocoObject { get; set; }
	}

	public class UiSoundObject
	{
		public string SoundName { get; set; }
		public RiffWavHeader Header { get; set; }
		public byte[] Data { get; set; }
	}

	public class UiSoundObjectList : IUiObject
	{
		public string FileName { get; set; }
		public List<UiSoundObject> Audio { get; set; } = [];
	}
}

using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Types;
using System.ComponentModel;

namespace OpenLoco.ObjectEditor.Gui
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IUiObject { }

	public interface IUiObjectWithGraphics
	{
		public BindingList<G1Element32> G1Elements { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiLocoObject : IUiObject, IUiObjectWithGraphics
	{
		public DatFileInfo DatFileInfo { get; set; }
		public ILocoObject LocoObject { get; set; }
		public BindingList<G1Element32> G1Elements
		{
			get => LocoObject.G1Elements;
			set => LocoObject.G1Elements = value;
		}
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
		public BindingList<UiSoundObject> Audio { get; set; } = [];
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UiG1 : IUiObject, IUiObjectWithGraphics
	{
		public G1Dat G1 { get; set; }

		public BindingList<G1Element32> G1Elements
		{
			get => G1.G1Elements;
			set
			{
				G1.G1Elements.Clear();
				foreach (var x in value)
				{
					G1.G1Elements.Add(x);
				}
			}
		}
	}
}

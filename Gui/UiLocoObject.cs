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
		public List<G1Element32> G1Elements { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record UiLocoObject(DatFileInfo DatFileInfo, ILocoObject? LocoObject) : IUiObject, IUiObjectWithGraphics
	{
		public List<G1Element32> G1Elements
		{
			get => LocoObject?.G1Elements ?? Enumerable.Empty<G1Element32>().ToList();
			set
			{
				if (LocoObject != null)
				{
					LocoObject.G1Elements = value;
				}
			}
		}
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record UiSoundObject(string SoundName)
	{
		public UiSoundObject(string soundName, RiffWavHeader header, byte[] data) : this(soundName)
		{
			Header = header;
			Data = data;
		}

		public RiffWavHeader Header { get; set; }
		public byte[] Data { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record UiSoundObjectList(string FileName) : IUiObject
	{
		public List<UiSoundObject> Audio { get; set; } = [];
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record UiG1(G1Dat G1) : IUiObject, IUiObjectWithGraphics
	{
		public List<G1Element32> G1Elements
		{
			get => G1?.G1Elements ?? Enumerable.Empty<G1Element32>().ToList();
			set
			{
				G1.G1Elements.Clear();
				G1.G1Elements.AddRange(value);
			}
		}
	}
}

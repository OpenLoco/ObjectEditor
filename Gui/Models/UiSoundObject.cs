using OpenLoco.Dat.Types;

namespace OpenLoco.Gui.Models
{
	public record UiSoundObject(string SoundName)
	{
		public UiSoundObject(string soundName, RiffWavHeader header, byte[] data) : this(soundName)
		{
			Header = header;
			Data = data;
			Duration = $"{data.Length / (decimal)header.ByteRate:0.#}s";
		}

		public string Duration { get; init; }

		public RiffWavHeader Header { get; set; }
		public byte[] Data { get; set; }
	}
}

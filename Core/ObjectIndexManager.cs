global using HeaderIndex = System.Collections.Generic.Dictionary<string, OpenLoco.ObjectEditor.DatFileParsing.ObjectIndex>;
using OpenLoco.ObjectEditor.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLoco.ObjectEditor
{
	public static class ObjectIndexManager
	{
		public static string SerialiseHeaderIndex(HeaderIndex headerIndex, ILogger? logger = null)
			=> JsonSerializer.Serialize(headerIndex, DefaultJsonSerializationOptions);

		public static HeaderIndex? DeserialiseHeaderIndex(string text, ILogger? logger = null)
			=> JsonSerializer.Deserialize<HeaderIndex>(text, DefaultJsonSerializationOptions);

		public static void SerialiseHeaderIndexToFile(string filename, HeaderIndex headerIndex, ILogger? logger = null)
		{
			logger?.Info($"Saved settings to {filename}");
			File.WriteAllText(filename, SerialiseHeaderIndex(headerIndex, logger));
		}

		public static HeaderIndex? DeserialiseHeaderIndexFromFile(string filename, ILogger? logger = null)
		{
			if (!File.Exists(filename))
			{
				logger?.Info($"Settings file {filename} does not exist");
				return null;
			}

			logger?.Info($"Loading settings from {filename}");
			return DeserialiseHeaderIndex(File.ReadAllText(filename), logger);
		}

		static readonly JsonSerializerOptions DefaultJsonSerializationOptions
			= new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };
	}
}

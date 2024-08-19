using OpenLoco.Dat.FileParsing;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenLoco.Common.Logging;

namespace OpenLoco.Dat
{
	public static class ObjectIndexManager
	{
		public static string SerialiseHeaderIndex(ObjectIndex headerIndex, ILogger? logger = null)
			=> JsonSerializer.Serialize(headerIndex, DefaultJsonSerializationOptions);

		public static ObjectIndex? DeserialiseHeaderIndex(string text, ILogger? logger = null)
			=> JsonSerializer.Deserialize<ObjectIndex>(text, DefaultJsonSerializationOptions);

		public static void SerialiseHeaderIndexToFile(string filename, ObjectIndex headerIndex, ILogger? logger = null)
		{
			logger?.Info($"Saved header index to {filename}");
			File.WriteAllText(filename, SerialiseHeaderIndex(headerIndex, logger));
		}

		public static ObjectIndex? DeserialiseHeaderIndexFromFile(string filename, ILogger? logger = null)
		{
			if (!File.Exists(filename))
			{
				logger?.Info($"Header index {filename} does not exist");
				return null;
			}

			logger?.Info($"Loading header index from {filename}");
			return DeserialiseHeaderIndex(File.ReadAllText(filename), logger);
		}

		static readonly JsonSerializerOptions DefaultJsonSerializationOptions
			= new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };
	}
}

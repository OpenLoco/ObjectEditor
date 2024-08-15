using OpenLoco.ObjectEditor.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared
{
	public static class Utils
	{
		public static Dictionary<string, ObjectMetadata> LoadMetadata(string filename)
		{
			if (!File.Exists(filename))
			{
				return [];
			}

			var text = File.ReadAllText(filename);
			var metadata = JsonSerializer.Deserialize<GlenDBSchema>(text);

			return metadata.data
				.DistinctBy(x => x.ObjectName) // this skips duplicates - todo - cleanup data instead
				.ToDictionary(
					x => x.ObjectName,
					x => new ObjectMetadata(x.ObjectName, 123) { Author = x.Creator, Description = x.DescriptionAndFile, Tags = x.Tags.ToList() });
		}

		public static ObjectMetadata LoadObjectMetadata(string objectName, uint checksum, Dictionary<string, ObjectMetadata> metadata)
		{
			if (!metadata.TryGetValue(objectName, out var value))
			{
				var text = File.ReadAllText(@"G:\\My Drive\\Locomotion\\Objects\\dataBase.json");
				var data = JsonSerializer.Deserialize<GlenDBSchema>(text); // this loads and deserialises the entire thing every time, rip
				var matching = data!.data.Where(x => x.ObjectName == objectName);
				var first = matching.FirstOrDefault();

				value = new ObjectMetadata(objectName, checksum);

				if (first != null)
				{
					value.Description = first.DescriptionAndFile;
					value.Author = first.Creator;
					//value.Tags.AddRange(first.Tags);

				}
				metadata.Add(objectName, value);
			}

			return value;
		}

		public static string GetDatCompositeKey(string name, uint checksum)
			=> $"{name.Trim()}_{checksum}";

		public static void SaveMetadata(string filename, Dictionary<string, ObjectMetadata> metadata)
		{
			var text = JsonSerializer.Serialize(metadata);

			var parentDir = Path.GetDirectoryName(filename);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			File.WriteAllText(filename, text);
		}
	}

	public class ObjectMetadata
	{
		public ObjectMetadata(string originalName, uint originalChecksum)
		{
			OriginalName = originalName;
			OriginalChecksum = originalChecksum;
		}

		public string OriginalName { get; }
		public uint OriginalChecksum { get; }
		public string Description { get; set; }
		public string Author { get; set; } = "<unknown>";
		public string Version { get; set; } // author-specified version
		public DateTimeOffset CreatedTime { get; set; } // creation UTC date is an implicit version
		public DateTimeOffset LastEditTime { get; set; } // last-edited UTC date is an implicit version

		[Browsable(false)]
		public List<string> Tags { get; set; } = [];
	}

	public class Data
	{
		[JsonPropertyName("C00")] public string ObjectName { get; set; }
		[JsonPropertyName("C01")] public string Image { get; set; }
		[JsonPropertyName("C02")] public string DescriptionAndFile { get; set; }
		[JsonPropertyName("C03")] public string ClassNumber { get; set; }
		[JsonPropertyName("C04")] public string Type { get; set; }
		[JsonPropertyName("C05")] public string TrackType { get; set; }
		[JsonPropertyName("C06")] public string Designed { get; set; }
		[JsonPropertyName("C07")] public string Obsolete { get; set; }
		[JsonPropertyName("C08")] public string Speed { get; set; }
		[JsonPropertyName("C09")] public string Power { get; set; }
		[JsonPropertyName("C10")] public string Weight { get; set; }
		[JsonPropertyName("C11")] public string Reliability { get; set; }
		[JsonPropertyName("C12")] public string Length { get; set; }
		[JsonPropertyName("C13")] public string NumCompat { get; set; }
		[JsonPropertyName("C14")] public string Sprites { get; set; }
		[JsonPropertyName("C15")] public string CargoCapacity1 { get; set; }
		[JsonPropertyName("C16")] public string CargoType1 { get; set; }
		[JsonPropertyName("C17")] public string CargoCapacity2 { get; set; }
		[JsonPropertyName("C18")] public string CargoType2 { get; set; }
		[JsonPropertyName("C19")] public string Creator { get; set; }
		[JsonPropertyName("C20")] public string _Tags { get; set; }

		public string[] Tags => _Tags.Split(" ");
	}

	public class GlenDBSchema
	{
		public IList<Data> data { get; set; }
	}
}

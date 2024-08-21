using System.Text.Json;

namespace OpenLoco.Common
{
	public static class Utils
	{
		public static List<ObjectMetadata> LoadMetadataList(string filename)
		{
			if (!File.Exists(filename))
			{
				return [];
			}

			var text = File.ReadAllText(filename);
			var metadata = JsonSerializer.Deserialize<GlenDBSchema>(text);

			return metadata.data
				.Select(x => new ObjectMetadata(x.ObjectName, 123) { Author = x.Creator, Description = x.DescriptionAndFile, Tags = [.. x.Tags] })
				.ToList();
		}

		public static Dictionary<string, ObjectMetadata> LoadMetadata(string filename)
		{
			if (!File.Exists(filename))
			{
				return [];
			}

			var text = File.ReadAllText(filename);
			var metadata = JsonSerializer.Deserialize<GlenDBSchema>(text);

			return metadata.data
				.DistinctBy(x => x.ObjectName) // todo: this skips duplicates, which may actually be valid - cleanup data instead with unique keys
				.ToDictionary(
					x => x.ObjectName,
					x => new ObjectMetadata(x.ObjectName, 123) { Author = x.Creator, Description = x.DescriptionAndFile, Tags = [.. x.Tags] });
		}

		public static ObjectMetadata? LoadObjectMetadata(string objectName, uint checksum, Dictionary<string, ObjectMetadata> metadata)
		{
			if (!metadata.TryGetValue(objectName, out var value))
			{
				const string metadataFile = "Q:\\Games\\Locomotion\\LocoVault\\dataBase.json";
				if (!File.Exists(metadataFile))
				{
					return null;
				}
				var text = File.ReadAllText(metadataFile);
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
			=> $"{name}_{checksum}";

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
}

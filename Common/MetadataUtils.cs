using System.Text.Json;

namespace OpenLoco.Common
{
	public static class MetadataUtils
	{
		public static List<Metadata> LoadMetadataList(string filename)
		{
			if (!File.Exists(filename))
			{
				return [];
			}

			var text = File.ReadAllText(filename);
			var metadata = JsonSerializer.Deserialize<GlenDBSchema>(text);

			return metadata.data
				.Select(x => new Metadata(x.ObjectName, 123) { Author = x.Creator, Description = x.DescriptionAndFile, Tags = [.. x.Tags] })
				.ToList();
		}

		public static Dictionary<string, Metadata> LoadMetadata(string filename)
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
					x => new Metadata(x.ObjectName, 123) { Author = x.Creator, Description = x.DescriptionAndFile, Tags = [.. x.Tags] });
		}

		public static Metadata? LoadObjectMetadata(string filename, string objectName, uint checksum, Dictionary<string, Metadata> metadata)
		{
			if (!metadata.TryGetValue(objectName, out var value))
			{
				if (!File.Exists(filename))
				{
					return null;
				}
				var text = File.ReadAllText(filename);
				var data = JsonSerializer.Deserialize<GlenDBSchema>(text); // this loads and deserialises the entire thing every time, rip
				var matching = data!.data.Where(x => x.ObjectName == objectName);
				var first = matching.FirstOrDefault();

				value = new Metadata(objectName, checksum);

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

		public static void SaveMetadata(string filename, Dictionary<string, Metadata> metadata)
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

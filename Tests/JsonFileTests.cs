using Common.Json;
using NUnit.Framework;

namespace Common.Tests;

[TestFixture]
public class JsonFileTests
{
	private record SampleRecord(string Name, int Value, List<string> Items);

	[Test]
	public async Task SerializeToFileAsync_WritesAtomicallyAndDeserializesCorrectly()
	{
		var tempPath = Path.Combine(Path.GetTempPath(), $"oe_test_{Guid.NewGuid():N}", "nested", "test.json");

		try
		{
			var sample = new SampleRecord("TestObject", 42, ["Item1", "Item2", "Item3"]);

			// Serialize (should automatically create directories and write via temporary atomic file)
			await JsonFile.SerializeToFileAsync(sample, tempPath);

			Assert.That(File.Exists(tempPath), Is.True);

			// Ensure no temporary files remained behind
			var dir = Path.GetDirectoryName(tempPath)!;
			var tmpFiles = Directory.GetFiles(dir, "*.tmp");
			Assert.That(tmpFiles, Is.Empty);

			// Deserialize
			var deserialized = await JsonFile.DeserializeFromFileAsync<SampleRecord>(tempPath);
			Assert.That(deserialized, Is.Not.Null);
			Assert.That(deserialized!.Name, Is.EqualTo(sample.Name));
			Assert.That(deserialized.Value, Is.EqualTo(sample.Value));
			Assert.That(deserialized.Items, Is.EqualTo(sample.Items));

			// Overwrite atomically with updated content
			var updated = new SampleRecord("UpdatedObject", 99, ["NewItem"]);
			await JsonFile.SerializeToFileAsync(updated, tempPath);

			var reloaded = await JsonFile.DeserializeFromFileAsync<SampleRecord>(tempPath);
			Assert.That(reloaded, Is.Not.Null);
			Assert.That(reloaded!.Name, Is.EqualTo("UpdatedObject"));
			Assert.That(reloaded.Value, Is.EqualTo(99));
		}
		finally
		{
			var parentDir = Path.GetDirectoryName(Path.GetDirectoryName(tempPath));
			if (!string.IsNullOrEmpty(parentDir) && Directory.Exists(parentDir))
			{
				try
				{
					Directory.Delete(parentDir, recursive: true);
				}
				catch
				{
					// Best-effort cleanup
				}
			}
		}
	}
}

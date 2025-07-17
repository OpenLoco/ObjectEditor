using Common.Logging;
using Definitions.Index;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ObjectIndexTests
{
	const string TestObjectFolder = "Q:\\Games\\Locomotion\\OriginalObjects\\GoG";
	const string TestObjectFolder2 = "Q:\\Games\\Locomotion\\OriginalObjects\\Steam";

	[Test]
	public void CreateIndexFile()
	{
		var logger = new Logger();
		var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".json");
		var index = ObjectIndex.LoadOrCreateIndex(tempFile, logger);

		Assert.That(index, Is.Not.Null);
		Assert.That(index.Indices, Has.Count.EqualTo(0));
	}

	[Test]
	public void CreateIndexFileFromDirectory()
	{
		var logger = new Logger();
		var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".json");
		var index = ObjectIndex.LoadOrCreateIndexFromDirectory(tempFile, TestObjectFolder, logger);

		Assert.That(index, Is.Not.Null);
		Assert.That(index.Indices, Has.Count.EqualTo(1));
		Assert.That(index.Indices.ContainsKey(TestObjectFolder), Is.True);
		Assert.That(index.Indices[TestObjectFolder], Has.Count.EqualTo(545));
		Assert.That(index.ObjectsIn(TestObjectFolder), Has.Count.EqualTo(545));
	}

	[Test]
	public void LoadOrCreateIndexDirectory()
	{
		var logger = new Logger();
		var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".json");
		var index = ObjectIndex.LoadOrCreateIndex(tempFile, logger);

		Assert.That(index.Indices, Has.Count.EqualTo(0));

		_ = index.LoadOrCreateIndexDirectory(TestObjectFolder, logger);

		Assert.That(index, Is.Not.Null);
		Assert.That(index.Indices, Has.Count.EqualTo(1));
		Assert.That(index.Indices.ContainsKey(TestObjectFolder), Is.True);
		Assert.That(index.Indices[TestObjectFolder], Has.Count.EqualTo(545));
		Assert.That(index.ObjectsIn(TestObjectFolder), Has.Count.EqualTo(545));
	}

	[Test]
	public void LoadOrCreateIndexDirectoryMultiple()
	{
		var logger = new Logger();
		var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".json");
		var index = ObjectIndex.LoadOrCreateIndex(tempFile, logger);

		_ = index.LoadOrCreateIndexDirectory(TestObjectFolder, logger);
		Assert.That(index.Indices, Has.Count.EqualTo(1));
		Assert.That(index.Indices.ContainsKey(TestObjectFolder), Is.True);
		Assert.That(index.Indices[TestObjectFolder], Has.Count.EqualTo(545));
		Assert.That(index.ObjectsIn(TestObjectFolder), Has.Count.EqualTo(545));

		_ = index.LoadOrCreateIndexDirectory(TestObjectFolder2, logger);
		Assert.That(index.Indices, Has.Count.EqualTo(2));
		Assert.That(index.Indices.ContainsKey(TestObjectFolder2), Is.True);
		Assert.That(index.Indices[TestObjectFolder2], Has.Count.EqualTo(544));
		Assert.That(index.ObjectsIn(TestObjectFolder2), Has.Count.EqualTo(544));
	}
}

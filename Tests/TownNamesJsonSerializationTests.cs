using System.Text.Json;
using Definitions.ObjectModels.Objects.TownNames;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class TownNamesJsonSerializationTests
{
	[Test]
	public void RoundTrip_SerializesAndDeserializesMorphemeCategories()
	{
		var original = new TownNamesMorphemesJson(
			Category1: new MorphemeCategory { Bias = 10, TownNames = [new StringTableEntry("Bridge", LocationFlags.None), new StringTableEntry("River", LocationFlags.AdjacentToSmallWaterBody)] },
			Category2: new MorphemeCategory { Bias = 5, TownNames = [new StringTableEntry("ton", LocationFlags.None)] },
			Category3: new MorphemeCategory { Bias = 0, TownNames = [] },
			Category4: new MorphemeCategory { Bias = 2, TownNames = [new StringTableEntry("Bay", LocationFlags.AdjacentToLargeWaterBody)] },
			Category5: new MorphemeCategory { Bias = 1, TownNames = [new StringTableEntry("Hill", LocationFlags.Mountainous)] },
			Category6: new MorphemeCategory { Bias = 8, TownNames = [new StringTableEntry("West", LocationFlags.None), new StringTableEntry("East", LocationFlags.None), new StringTableEntry("North", LocationFlags.None)] }
		);

		var json = JsonSerializer.Serialize(original, new JsonSerializerOptions { WriteIndented = true });
		TestContext.WriteLine($"Serialized JSON:\n{json}");

		var deserialized = JsonSerializer.Deserialize<TownNamesMorphemesJson>(json);

		Assert.That(deserialized, Is.Not.Null);

		// Check Category1
		Assert.That(deserialized!.Category1.Bias, Is.EqualTo((byte)10));
		Assert.That(deserialized.Category1.TownNames, Has.Count.EqualTo(2));
		Assert.That(deserialized.Category1.TownNames[0].Text, Is.EqualTo("Bridge"));
		Assert.That(deserialized.Category1.TownNames[0].LocationHint, Is.EqualTo(LocationFlags.None));
		Assert.That(deserialized.Category1.TownNames[1].Text, Is.EqualTo("River"));
		Assert.That(deserialized.Category1.TownNames[1].LocationHint, Is.EqualTo(LocationFlags.AdjacentToSmallWaterBody));

		// Check Category2
		Assert.That(deserialized.Category2.Bias, Is.EqualTo((byte)5));
		Assert.That(deserialized.Category2.TownNames, Has.Count.EqualTo(1));
		Assert.That(deserialized.Category2.TownNames[0].LocationHint, Is.EqualTo(LocationFlags.None));

		// Check Category3 (empty)
		Assert.That(deserialized.Category3.TownNames, Has.Count.EqualTo(0));

		// Check Category4
		Assert.That(deserialized.Category4.TownNames, Has.Count.EqualTo(1));
		Assert.That(deserialized.Category4.TownNames[0].Text, Is.EqualTo("Bay"));
		Assert.That(deserialized.Category4.TownNames[0].LocationHint, Is.EqualTo(LocationFlags.AdjacentToLargeWaterBody));

		// Check Category5
		Assert.That(deserialized.Category5.TownNames, Has.Count.EqualTo(1));
		Assert.That(deserialized.Category5.TownNames[0].LocationHint, Is.EqualTo(LocationFlags.Mountainous));

		// Check Category6
		Assert.That(deserialized.Category6.TownNames, Has.Count.EqualTo(3));
		Assert.That(deserialized.Category6.TownNames[2].Text, Is.EqualTo("North"));
	}
}

using Definitions.Database;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Types;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests;

/// <summary>
/// Verifies that the complex object sub-table properties round-trip through the JSON value converter and that
/// EF Core maps them to SQLite <c>json</c> columns.
/// </summary>
[TestFixture]
public class ObjectSubTableJsonStorageTests
{
	private static T RoundTrip<T>(T value)
		where T : notnull
		=> JsonValueConverter<T>.Deserialize(JsonValueConverter<T>.Serialize(value));

	[Test]
	public void RoundTrip_ObjectModelHeaderList()
	{
		List<ObjectModelHeader> value =
		[
			new ObjectModelHeader("TRACKST", ObjectType.Track, ObjectSource.Custom, 123456),
		];

		var result = RoundTrip(value);

		Assert.That(result, Has.Count.EqualTo(1));
		Assert.That(result[0].Name, Is.EqualTo("TRACKST"));
		Assert.That(result[0].ObjectType, Is.EqualTo(ObjectType.Track));
		Assert.That(result[0].ObjectSource, Is.EqualTo(ObjectSource.Custom));
		Assert.That(result[0].DatChecksum, Is.EqualTo(123456u));
	}

	[Test]
	public void RoundTrip_BuildingComponents()
	{
		var value = new BuildingComponents
		{
			BuildingHeights = [1, 2, 4],
			BuildingAnimations =
			[
				new BuildingPartAnimation { NumFrames = 2, AnimationSpeed = 1 },
				new BuildingPartAnimation { NumFrames = 4, AnimationSpeed = 3 },
				new BuildingPartAnimation { NumFrames = 8, AnimationSpeed = 7 },
			],
			BuildingVariations = [[0, 1], [2]],
		};

		var result = RoundTrip(value);

		Assert.That(result.BuildingHeights, Is.EqualTo(new byte[] { 1, 2, 4 }));
		Assert.That(result.BuildingAnimations, Has.Count.EqualTo(3));
		Assert.That(result.BuildingAnimations[1].NumFrames, Is.EqualTo((byte)4));
		Assert.That(result.BuildingVariations, Has.Count.EqualTo(2));
		Assert.That(result.BuildingVariations[0], Is.EqualTo(new byte[] { 0, 1 }));
	}

	[Test]
	public void RoundTrip_NestedListsAndArrays()
	{
		// List<List<uint8_t>> (e.g. Industry.AnimationSequences)
		var animationSequences = new List<List<byte>>
		{
			new() { 1, 2 },
			new() { 3, 4, 5 },
		};
		var animationResult = RoundTrip(animationSequences);
		Assert.That(animationResult, Has.Count.EqualTo(2));
		Assert.That(animationResult[1], Is.EqualTo(new byte[] { 3, 4, 5 }));

		// CargoOffset[][][] (e.g. RoadStation.CargoOffsets)
		var cargoOffsets = new CargoOffset[][][]
		{
			new CargoOffset[][]
			{
				new CargoOffset[]
				{
					new() { A = new Pos3 { X = 1, Y = 2, Z = 3 }, B = new Pos3 { X = 4, Y = 5, Z = 6 } },
				},
				[],
			},
		};
		var cargoResult = RoundTrip(cargoOffsets);
		Assert.That(cargoResult, Has.Length.EqualTo(1));
		Assert.That(cargoResult[0][0][0].A.Z, Is.EqualTo((short)3));
		Assert.That(cargoResult[0][0][0].B.X, Is.EqualTo((short)4));

		// uint8_t[][] (e.g. TrackStation.DiagonalCargoOffsetBytes)
		var jaggedBytes = new byte[][]
		{
			new byte[] { 1, 2 },
			[],
			new byte[] { 3 },
		};
		var byteResult = RoundTrip(jaggedBytes);
		Assert.That(byteResult, Has.Length.EqualTo(3));
		Assert.That(byteResult[2], Is.EqualTo(new byte[] { 3 }));
	}

	[Test]
	public void RoundTrip_VehicleCargoStructures()
	{
		// List<CargoCategory>[] (e.g. Vehicle.CompatibleCargoCategories)
		var compatibleCargo = new List<CargoCategory>[2]
		{
			new() { CargoCategory.Passengers },
			new(),
		};
		var cargoResult = RoundTrip(compatibleCargo);
		Assert.That(cargoResult, Has.Length.EqualTo(2));
		Assert.That(cargoResult[0], Is.EqualTo(new[] { CargoCategory.Passengers }));
		Assert.That(cargoResult[1], Is.Empty);

		// Dictionary<CargoCategory, uint8_t> (e.g. Vehicle.CargoTypeSpriteOffsets)
		var spriteOffsets = new Dictionary<CargoCategory, byte> { [CargoCategory.Passengers] = 3, [CargoCategory.Coal] = 9 };
		var spriteResult = RoundTrip(spriteOffsets);
		Assert.That(spriteResult[CargoCategory.Passengers], Is.EqualTo((byte)3));
		Assert.That(spriteResult[CargoCategory.Coal], Is.EqualTo((byte)9));
	}

	[Test]
	public void RoundTrip_EnumList()
	{
		// List<CargoInfluenceTownFilterType> (e.g. Region.CargoInfluenceTownFilter)
		var value = new List<CargoInfluenceTownFilterType> { CargoInfluenceTownFilterType.AllTowns };
		var result = RoundTrip(value);
		Assert.That(result, Is.EqualTo(value));
	}

	[Test]
	public void RoundTrip_MorphemeCategories()
	{
		// List<MorphemeCategory> (e.g. TownNames.MorphemeCategories)
		var value = new List<MorphemeCategory>
		{
			new() { Bias = 7, TownNames = [new StringTableEntry("Bridge", LocationFlags.None)] },
		};

		var result = RoundTrip(value);
		Assert.That(result, Has.Count.EqualTo(1));
		Assert.That(result[0].Bias, Is.EqualTo((byte)7));
		Assert.That(result[0].TownNames[0].Text, Is.EqualTo("Bridge"));
	}

	[Test]
	public void RoundTrip_Pos2()
	{
		var result = RoundTrip(new Pos2 { X = 10, Y = 20 });
		Assert.That(result.X, Is.EqualTo((short)10));
		Assert.That(result.Y, Is.EqualTo((short)20));
	}

	[Test]
	public void Deserialize_EmptyStoredValue_ReturnsEmptyCollectionOrObject()
	{
		// Columns added to an existing table get a '' default from the SQLite provider.
		Assert.That(JsonValueConverter<List<ObjectModelHeader>>.Deserialize(string.Empty), Is.Empty);
		Assert.That(JsonValueConverter<byte[][]>.Deserialize(string.Empty), Is.Empty);
		Assert.That(JsonValueConverter<List<CargoCategory>[]>.Deserialize(string.Empty), Is.Empty);
		Assert.That(JsonValueConverter<BuildingComponents>.Deserialize(string.Empty), Is.Not.Null);
	}

	[Test]
	public void Model_MapsComplexSubObjectPropertiesToJsonColumns()
	{
		using var db = new LocoDbContext();
		var model = db.Model;

		AssertJsonColumn<TblObjectVehicle>(model, nameof(TblObjectVehicle.BodySprites));
		AssertJsonColumn<TblObjectVehicle>(model, nameof(TblObjectVehicle.CompatibleCargoCategories));
		AssertJsonColumn<TblObjectVehicle>(model, nameof(TblObjectVehicle.CargoTypeSpriteOffsets));
		AssertJsonColumn<TblObjectVehicle>(model, nameof(TblObjectVehicle.MaxCargo));
		AssertJsonColumn<TblObjectAirport>(model, nameof(TblObjectAirport.BuildingComponents));
		AssertJsonColumn<TblObjectAirport>(model, nameof(TblObjectAirport.MovementNodes));
		AssertJsonColumn<TblObjectIndustry>(model, nameof(TblObjectIndustry.InitialProductionRate));
		AssertJsonColumn<TblObjectRoadStation>(model, nameof(TblObjectRoadStation.CargoOffsets));
		AssertJsonColumn<TblObjectTownNames>(model, nameof(TblObjectTownNames.MorphemeCategories));
		AssertJsonColumn<TblObjectTrackStation>(model, nameof(TblObjectTrackStation.DiagonalCargoOffsetBytes));
	}

	private static void AssertJsonColumn<TEntity>(Microsoft.EntityFrameworkCore.Metadata.IModel model, string propertyName)
	{
		var property = model.FindEntityType(typeof(TEntity))?.FindProperty(propertyName);

		Assert.That(property, Is.Not.Null, $"{typeof(TEntity).Name}.{propertyName} was not mapped.");
		Assert.That(property!.GetColumnType(), Is.EqualTo(JsonColumnConvention.ColumnType), $"{typeof(TEntity).Name}.{propertyName} is not a json column.");
	}
}

using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using NUnit.Framework;
using Shared.Validation;

namespace Dat.Tests;

[TestFixture]
public class ObjectValidationTests
{
	static ObjectModelHeader Make(string name, ObjectType type, uint checksum)
		=> new(name, type, ObjectSource.Custom, checksum);

	static ObjectModelHeader MakeVanilla(string name, ObjectType type, uint checksum)
		=> new(name, type, ObjectSource.LocomotionSteam, checksum);

	static ObjectModelHeader MakeOpenLoco(string name, ObjectType type, uint checksum)
		=> new(name, type, ObjectSource.OpenLoco, checksum);

	/// <summary>
	/// A resolver that mimics an industry object's cargo dependencies, keyed by the industry name.
	/// </summary>
	static Func<ObjectModelHeader, IEnumerable<ObjectModelHeader>> IndustryResolver(
		Dictionary<string, IEnumerable<ObjectModelHeader>> dependencies)
		=> header => dependencies.TryGetValue(header.Name, out var deps) ? deps : [];

	[Test]
	public void ValidateScenario_PassesWhenAllCargoDependenciesAreIncluded()
	{
		var industry = Make("COAL_MINE", ObjectType.Industry, 0x1111);
		var produced = Make("COAL", ObjectType.Cargo, 0x2222);
		var consumed = Make("WOOD", ObjectType.Cargo, 0x3333);

		var scenarioObjects = new[] { industry, produced, consumed };
		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [produced, consumed],
		});

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, resolver);

		Assert.That(errors, Is.Empty);
	}

	[Test]
	public void ValidateScenario_ReportsMissingConsumedCargo()
	{
		var industry = Make("COAL_MINE", ObjectType.Industry, 0x1111);
		var produced = Make("COAL", ObjectType.Cargo, 0x2222);

		// The scenario includes the produced cargo but not the consumed cargo "WOOD".
		var scenarioObjects = new[] { industry, produced };
		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [produced, Make("WOOD", ObjectType.Cargo, 0x3333)],
		});

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, resolver);

		Assert.That(errors, Has.Count.EqualTo(1));
		Assert.That(errors[0], Does.Contain("COAL_MINE"));
		Assert.That(errors[0], Does.Contain("WOOD"));
	}

	[Test]
	public void ValidateScenario_ReportsEachMissingDependencyOncePerObject()
	{
		var industry = Make("COAL_MINE", ObjectType.Industry, 0x1111);

		// Neither produced nor consumed cargo is present.
		var scenarioObjects = new[] { industry };
		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [
				Make("COAL", ObjectType.Cargo, 0x2222),
				Make("WOOD", ObjectType.Cargo, 0x3333),
			],
		});

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, resolver);

		Assert.That(errors, Has.Count.EqualTo(2));
		Assert.That(errors, Has.Some.Contains("COAL"));
		Assert.That(errors, Has.Some.Contains("WOOD"));
	}

	[Test]
	public void ValidateScenario_DoesNotDuplicateErrorsWhenObjectAppearsTwice()
	{
		var industry = Make("COAL_MINE", ObjectType.Industry, 0x1111);
		var produced = Make("COAL", ObjectType.Cargo, 0x2222);

		// industry listed twice; still only a single missing "WOOD" report expected.
		var scenarioObjects = new[] { industry, industry, produced };
		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [produced, Make("WOOD", ObjectType.Cargo, 0x3333)],
		});

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, resolver);

		Assert.That(errors, Has.Count.EqualTo(1));
		Assert.That(errors[0], Does.Contain("WOOD"));
	}

	[Test]
	public void ValidateScenario_PassesForObjectsWithoutDependencies()
	{
		var water = Make("WATER", ObjectType.Water, 0x4444);
		var scenarioObjects = new[] { water };

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, IndustryResolver([]));

		Assert.That(errors, Is.Empty);
	}

	[Test]
	public void ValidateScenario_IgnoresEmptyPlaceholderObjects()
	{
		var industry = Make("COAL_MINE", ObjectType.Industry, 0x1111);
		var produced = Make("COAL", ObjectType.Cargo, 0x2222);
		var placeholder = new ObjectModelHeader(); // empty name -> treated as a fill/empty slot

		var scenarioObjects = new[] { industry, produced, placeholder };
		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [produced, Make("WOOD", ObjectType.Cargo, 0x3333)],
		});

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, resolver);

		Assert.That(errors, Has.Count.EqualTo(1));
		Assert.That(errors[0], Does.Contain("WOOD"));
	}

	[Test]
	public void ValidateSCV5_VanillaDependencyMatchesIncludedObjectByNameIgnoringChecksum()
	{
		var industry = MakeVanilla("COAL_MINE", ObjectType.Industry, 0x1111);
		var produced = MakeVanilla("COAL", ObjectType.Cargo, 0xAAAA);
		var consumed = MakeVanilla("WOOD", ObjectType.Cargo, 0xBBBB);

		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [produced, consumed],
		});

		// The scenario records checksums for the same vanilla object names that differ from the
		// dependency headers' recorded checksums (e.g. Steam vs GoG). Vanilla must still match by name.
		var scenarioObjectsWithDifferentChecksums = new[]
		{
			industry,
			MakeVanilla("COAL", ObjectType.Cargo, 0x1111),
			MakeVanilla("WOOD", ObjectType.Cargo, 0x2222),
		};

		var errors = ObjectValidation.ValidateSCV5(scenarioObjectsWithDifferentChecksums, resolver);

		Assert.That(errors, Is.Empty, "Vanilla objects should match by name even when checksums differ");
	}

	[Test]
	public void ValidateSCV5_CustomDependencyDoesNotMatchIncludedObjectWithDifferentChecksum()
	{
		var industry = Make("COAL_MINE", ObjectType.Industry, 0x1111);
		var produced = Make("COAL", ObjectType.Cargo, 0xAAAA);
		var consumed = Make("WOOD", ObjectType.Cargo, 0xBBBB);

		// The scenario includes a custom "WOOD" but with a different checksum.
		var scenarioObjects = new[] { industry, produced, Make("WOOD", ObjectType.Cargo, 0x2222) };
		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [produced, consumed],
		});

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, resolver);

		Assert.That(errors, Has.Count.EqualTo(1));
		Assert.That(errors[0], Does.Contain("WOOD"));
	}

	[Test]
	public void ValidateSCV5_OpenLocoDependencyMatchesByNameAndChecksum()
	{
		var industry = MakeOpenLoco("COAL_MINE", ObjectType.Industry, 0x1111);
		var produced = MakeOpenLoco("COAL", ObjectType.Cargo, 0xAAAA);
		var consumed = MakeOpenLoco("WOOD", ObjectType.Cargo, 0xBBBB);

		// Scenario includes a different-checksum "WOOD" - should NOT match for OpenLoco.
		var scenarioObjects = new[] { industry, produced, MakeOpenLoco("WOOD", ObjectType.Cargo, 0x2222) };
		var resolver = IndustryResolver(new Dictionary<string, IEnumerable<ObjectModelHeader>>
		{
			[industry.Name] = [produced, consumed],
		});

		var errors = ObjectValidation.ValidateSCV5(scenarioObjects, resolver);

		Assert.That(errors, Has.Count.EqualTo(1));
		Assert.That(errors[0], Does.Contain("WOOD"));
	}

	[Test]
	public void GetObjectDependencies_ReturnsProducedAndRequiredCargoForIndustry()
	{
		var industry = new IndustryObject
		{
			ProducedCargo = [Make("COAL", ObjectType.Cargo, 0x2222)],
			RequiredCargo = [Make("WOOD", ObjectType.Cargo, 0x3333)],
		};

		var deps = ObjectValidation.GetObjectDependencies(industry).ToList();

		Assert.That(deps, Has.Count.EqualTo(2));
		Assert.That(deps.Select(x => x.Name), Is.EquivalentTo(new[] { "COAL", "WOOD" }));
	}

	[Test]
	public void GetObjectDependencies_ReturnsEmptyForNonIndustryObject()
	{
		Assert.That(ObjectValidation.GetObjectDependencies(null), Is.Empty);
	}

	[Test]
	public void GetObjectDependencies_BuildingReturnsProducedAndConsumedCargo()
	{
		var building = new BuildingObject
		{
			ProducedCargoType = [Make("COAL", ObjectType.Cargo, 0x10)],
			ConsumedCargoType = [Make("WOOD", ObjectType.Cargo, 0x20)],
		};

		var deps = ObjectValidation.GetObjectDependencies(building).Select(x => x.Name).ToList();

		Assert.That(deps, Is.EquivalentTo(new[] { "COAL", "WOOD" }));
	}

	[Test]
	public void GetObjectDependencies_BridgeReturnsCompatibleTrackAndRoad()
	{
		var bridge = new BridgeObject
		{
			CompatibleTrackObjects = [Make("TRACK1", ObjectType.Track, 0x10)],
			CompatibleRoadObjects = [Make("ROAD1", ObjectType.Road, 0x20)],
		};

		var deps = ObjectValidation.GetObjectDependencies(bridge).Select(x => x.Name).ToList();

		Assert.That(deps, Is.EquivalentTo(new[] { "TRACK1", "ROAD1" }));
	}

	[Test]
	public void GetObjectDependencies_LandReturnsCliffEdgeAndOptionalReplacement()
	{
		var landWithReplacement = new LandObject
		{
			CliffEdgeHeader = Make("CLIFF", ObjectType.CliffEdge, 0x10),
			ReplacementLandHeader = Make("LAND2", ObjectType.Land, 0x20),
		};
		var landWithoutReplacement = new LandObject { CliffEdgeHeader = Make("CLIFF", ObjectType.CliffEdge, 0x10) };

		Assert.That(ObjectValidation.GetObjectDependencies(landWithReplacement).Select(x => x.Name),
			Is.EquivalentTo(new[] { "CLIFF", "LAND2" }));
		Assert.That(ObjectValidation.GetObjectDependencies(landWithoutReplacement).Select(x => x.Name),
			Is.EquivalentTo(new[] { "CLIFF" }));
	}

	[Test]
	public void GetObjectDependencies_RegionReturnsCargoInfluenceAndDependentObjects()
	{
		var region = new RegionObject
		{
			CargoInfluenceObjects = [Make("COAL", ObjectType.Cargo, 0x10)],
			DependentObjects = [Make("DEP", ObjectType.Sound, 0x20)],
		};

		var deps = ObjectValidation.GetObjectDependencies(region).Select(x => x.Name).ToList();

		Assert.That(deps, Is.EquivalentTo(new[] { "COAL", "DEP" }));
	}

	[Test]
	public void GetObjectDependencies_RoadReturnsAllReferences()
	{
		var road = new RoadObject
		{
			Tunnel = Make("TUN1", ObjectType.Tunnel, 0x10),
			Bridges = [Make("BR1", ObjectType.Bridge, 0x20)],
			Stations = [Make("ST1", ObjectType.RoadStation, 0x30)],
			RoadMods = [Make("RM1", ObjectType.RoadExtra, 0x40)],
			TracksAndRoads = [Make("TR1", ObjectType.Track, 0x50)],
		};

		var deps = ObjectValidation.GetObjectDependencies(road).Select(x => x.Name).ToList();

		Assert.That(deps, Is.EquivalentTo(new[] { "TUN1", "BR1", "ST1", "RM1", "TR1" }));
	}

	[Test]
	public void GetObjectDependencies_RoadStationReturnsCompatibleRoadsAndOptionalCargo()
	{
		var withCargo = new RoadStationObject
		{
			CompatibleRoadObjects = [Make("ROAD1", ObjectType.Road, 0x10)],
			CargoType = Make("COAL", ObjectType.Cargo, 0x20),
		};
		var withoutCargo = new RoadStationObject { CompatibleRoadObjects = [Make("ROAD1", ObjectType.Road, 0x10)] };

		Assert.That(ObjectValidation.GetObjectDependencies(withCargo).Select(x => x.Name),
			Is.EquivalentTo(new[] { "ROAD1", "COAL" }));
		Assert.That(ObjectValidation.GetObjectDependencies(withoutCargo).Select(x => x.Name),
			Is.EquivalentTo(new[] { "ROAD1" }));
	}

	[Test]
	public void GetObjectDependencies_SteamReturnsSoundEffects()
	{
		var steam = new SteamObject { SoundEffects = [Make("SND1", ObjectType.Sound, 0x10)] };

		var deps = ObjectValidation.GetObjectDependencies(steam).Select(x => x.Name).ToList();

		Assert.That(deps, Is.EquivalentTo(new[] { "SND1" }));
	}

	[Test]
	public void GetObjectDependencies_TrackReturnsAllReferences()
	{
		var track = new TrackObject
		{
			Tunnel = Make("TUN1", ObjectType.Tunnel, 0x10),
			Bridges = [Make("BR1", ObjectType.Bridge, 0x20)],
			Stations = [Make("ST1", ObjectType.TrackStation, 0x30)],
			Signals = [Make("SG1", ObjectType.TrackSignal, 0x40)],
			TrackMods = [Make("TM1", ObjectType.TrackExtra, 0x50)],
			TracksAndRoads = [Make("ROAD1", ObjectType.Road, 0x60)],
		};

		var deps = ObjectValidation.GetObjectDependencies(track).Select(x => x.Name).ToList();

		Assert.That(deps, Is.EquivalentTo(new[] { "TUN1", "BR1", "ST1", "SG1", "TM1", "ROAD1" }));
	}

	[Test]
	public void GetObjectDependencies_TrackSignalAndTrackStationReturnCompatibleTracks()
	{
		Assert.That(ObjectValidation.GetObjectDependencies(new TrackSignalObject { CompatibleTrackObjects = [Make("TR1", ObjectType.Track, 0x10)] }).Select(x => x.Name),
			Is.EquivalentTo(new[] { "TR1" }));
		Assert.That(ObjectValidation.GetObjectDependencies(new TrackStationObject { CompatibleTrackObjects = [Make("TR1", ObjectType.Track, 0x10)] }).Select(x => x.Name),
			Is.EquivalentTo(new[] { "TR1" }));
	}

	[Test]
	public void GetObjectDependencies_VehicleReturnsAllDependencyTypes()
	{
		var vehicle = new VehicleObject
		{
			RoadOrTrackType = Make("TR1", ObjectType.Track, 0x10),
			CompatibleVehicles = [Make("V2", ObjectType.Vehicle, 0x20)],
			RequiredTrackExtras = [Make("TM1", ObjectType.TrackExtra, 0x30)],
			RackRail = Make("RR1", ObjectType.TrackExtra, 0x40),
			DrivingSound = Make("SND1", ObjectType.Sound, 0x50),
			StartSounds = [Make("START1", ObjectType.Sound, 0x60)],
			CrossingSounds = [Make("CROSS1", ObjectType.Sound, 0x70)],
			FrictionSound = new FrictionSound { SoundObject = Make("FRICTION", ObjectType.Sound, 0x80) },
			SimpleMotorSound = new SimpleMotorSound { SoundObject = Make("MOTOR", ObjectType.Sound, 0x90) },
			GearboxMotorSound = new GearboxMotorSound { SoundObject = Make("GEARBOX", ObjectType.Sound, 0xA0) },
			ParticleEmitters = [new EmitterAnimation { AnimationObject = Make("STEAM1", ObjectType.Steam, 0xB0) }],
		};

		var deps = ObjectValidation.GetObjectDependencies(vehicle).Select(x => x.Name).ToList();

		Assert.That(deps, Is.EquivalentTo(new[]
		{
			"TR1", "V2", "TM1", "RR1", "SND1", "START1", "CROSS1", // direct
			"FRICTION", "MOTOR", "GEARBOX", // nested sound objects
			"STEAM1", // particle emitter animation object
		}));
	}

	[Test]
	public void GetObjectDependencies_VehicleWithoutOptionalDependenciesIsMinimal()
	{
		// A bare vehicle with no optional fields set should produce no noise or null-reference errors.
		var vehicle = new VehicleObject();

		var deps = ObjectValidation.GetObjectDependencies(vehicle).ToList();

		Assert.That(deps, Is.Empty);
	}
}
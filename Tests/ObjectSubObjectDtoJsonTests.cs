using Definitions.Database;
using Definitions.DTO;
using NUnit.Framework;
using System.Text.Json;

namespace Tests;

/// <summary>
/// Confirms that every object sub-table has a matching DTO that exposes all of its properties (i.e. the
/// object service DTOs can return the full object JSON), and that those DTOs round-trip through the
/// polymorphic JSON contract used by the object descriptor API.
/// </summary>
[TestFixture]
public class ObjectSubObjectDtoJsonTests
{
	static readonly System.Reflection.Assembly DefinitionsAssembly = typeof(TblObject).Assembly;

	static IEnumerable<Type> TblSubObjectTypes()
		=> DefinitionsAssembly.GetTypes()
			.Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IDbSubObject).IsAssignableFrom(t));

	static Dictionary<string, Type> PublicInstanceProperties(Type type)
		=> type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
			.ToDictionary(p => p.Name, p => p.PropertyType);

	[Test]
	public void EveryObjectTypeHasAMatchingDtoWithAllProperties()
	{
		var tblTypes = TblSubObjectTypes().ToList();
		Assert.That(tblTypes, Has.Count.EqualTo(34), "There should be one sub-table per object type.");

		using (Assert.EnterMultipleScope())
		{
			foreach (var tblType in tblTypes)
			{
				var dtoType = DefinitionsAssembly.GetType($"Definitions.DTO.Dto{tblType.Name[3..]}"); // TblObjectX -> DtoObjectX
				Assert.That(dtoType, Is.Not.Null, $"Missing DTO for {tblType.Name}.");

				var tblProps = PublicInstanceProperties(tblType);
				tblProps.Remove(nameof(DbSubObject.Parent)); // parent is the FK navigation, not object data
				var dtoProps = PublicInstanceProperties(dtoType!);

				Assert.That(dtoProps.Keys, Is.EquivalentTo(tblProps.Keys), $"{dtoType!.Name} property names differ from {tblType.Name}.");

				foreach (var (name, type) in tblProps)
				{
					Assert.That(dtoProps.TryGetValue(name, out var dtoPropType), Is.True, $"{dtoType!.Name}.{name} is missing.");
					Assert.That(dtoPropType, Is.EqualTo(type), $"{dtoType!.Name}.{name} has a different type to {tblType.Name}.{name}.");
				}
			}
		}
	}

	[Test]
	public void EverySubObjectDtoRoundTripsPolymorphically()
	{
		using (Assert.EnterMultipleScope())
		{
			foreach (var tblType in TblSubObjectTypes())
			{
				var dtoType = DefinitionsAssembly.GetType($"Definitions.DTO.Dto{tblType.Name[3..]}"); // TblObjectX -> DtoObjectX
				Assert.That(dtoType, Is.Not.Null, $"Missing DTO for {tblType.Name}.");

				var instance = (IDtoSubObject)Activator.CreateInstance(dtoType!)!;
				var json = JsonSerializer.Serialize(instance);

				Assert.That(json, Does.Contain("\"$type\""), $"{dtoType!.Name} did not serialize a $type discriminator.");

				var roundTripped = JsonSerializer.Deserialize<IDtoSubObject>(json);
				Assert.That(roundTripped, Is.TypeOf(dtoType!), $"{dtoType!.Name} did not round-trip to the same type.");
			}
		}
	}

	[Test]
	public void ObjectDescriptorSerializesItsSubObject()
	{
		var descriptor = new DtoObjectPostResponse(
			Id: 42,
			Name: "internal-name",
			DisplayName: "Display Name",
			DatChecksum: 123,
			Description: null,
			ObjectSource: Definitions.ObjectModels.Types.ObjectSource.Custom,
			ObjectType: Definitions.ObjectModels.Types.ObjectType.Airport,
			VehicleType: null,
			Availability: Definitions.ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: new DateOnly(2026, 1, 1),
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor(new Dictionary<string, Dictionary<Definitions.ObjectModels.Types.LanguageId, string>>(), 42),
			SubObject: new DtoObjectAirport { Id = 7, MinX = -3, RequiredClearEdges = 9 });

		var json = JsonSerializer.Serialize(descriptor);
		Assert.That(json, Does.Contain("\"$type\":\"airport\""));

		var roundTripped = JsonSerializer.Deserialize<DtoObjectPostResponse>(json);
		Assert.That(roundTripped, Is.Not.Null);
		Assert.That(roundTripped!.SubObject, Is.TypeOf<DtoObjectAirport>());
		Assert.That(((DtoObjectAirport)roundTripped.SubObject!).RequiredClearEdges, Is.EqualTo(9u));
	}
}

using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using NUnit.Framework;

namespace Tests;

/// <summary>
/// Guards the DTO → sub-object table mapping used by <c>PUT /v2/objects/{id}</c>: every sub-object DTO
/// must have exactly one reverse mapper (the lookup is convention-based), so a newly added object type
/// fails loudly instead of silently dropping its properties.
/// </summary>
[TestFixture]
public class SubObjectDtoMapperTests
{
	/// <summary>Every concrete DTO that can appear as <c>DtoObjectPostResponse.SubObject</c>.</summary>
	static IReadOnlyList<Type> DtoSubObjectTypes => [.. typeof(IDtoSubObject).Assembly
		.GetTypes()
		.Where(t => t.IsClass && !t.IsAbstract && typeof(IDtoSubObject).IsAssignableFrom(t))
		.OrderBy(t => t.Name)];

	[Test]
	public void EveryDtoSubObjectType_HasExactlyOneReverseMapper()
	{
		var dtoTypes = DtoSubObjectTypes;
		Assert.That(dtoTypes, Is.Not.Empty);

		var withoutMapper = dtoTypes
			.Where(t => SubObjectDtoMapper.FindMapperOrNull(t) is null)
			.Select(t => t.Name)
			.ToList();

		Assert.That(withoutMapper, Is.Empty, "sub-object DTO types without exactly one reverse mapper");
	}

	[Test]
	public void ToTableEntity_MapsPropertiesAndParent()
	{
		var parent = new TblObject
		{
			Id = 42,
			Name = "parent",
			ObjectType = ObjectType.Airport,
			ObjectSource = ObjectSource.Custom,
			Availability = ObjectAvailability.Available,
		};

		var dto = new DtoObjectAirport
		{
			Id = 7,
			BuildCostFactor = 123,
			MinX = -3,
			RequiredClearEdges = 9,
		};

		var entity = SubObjectDtoMapper.ToTableEntity(dto, parent);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(entity, Is.TypeOf<TblObjectAirport>());

			var airport = (TblObjectAirport)entity;
			Assert.That(airport.BuildCostFactor, Is.EqualTo((short)123));
			Assert.That(airport.MinX, Is.EqualTo((sbyte)-3));
			Assert.That(airport.RequiredClearEdges, Is.EqualTo(9u));
			Assert.That(airport.Parent, Is.SameAs(parent));
			Assert.That(airport.Id, Is.EqualTo(7ul));
		}
	}
}
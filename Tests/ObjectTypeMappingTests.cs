using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using NUnit.Framework;

namespace Tests;

/// <summary>
/// Guards the canonical <see cref="ObjectType"/> ⇄ CLR struct-type mapping (now the only copy shared by
/// the object service, the GUI and DatabaseTools).
/// </summary>
[TestFixture]
public class ObjectTypeMappingTests
{
	[Test]
	public void StructTypeMapping_RoundTripsForEveryObjectType()
	{
		foreach (var objectType in Enum.GetValues<ObjectType>())
		{
			var structType = ObjectTypeMapping.ObjectTypeToStructType(objectType);

			Assert.That(structType, Is.Not.Null, objectType.ToString());
			Assert.That(ObjectTypeMapping.StructTypeToObjectType(structType), Is.EqualTo(objectType), objectType.ToString());
		}
	}

	[Test]
	public void StructTypeToObjectType_ThrowsForUnknownType()
	{
		_ = Assert.Throws<ArgumentOutOfRangeException>(() => ObjectTypeMapping.StructTypeToObjectType(typeof(string)));
	}

	[Test]
	public void ObjectTypeToStructType_ThrowsForUndefinedObjectType()
	{
		var highestDefined = Convert.ToInt32(Enum.GetValues<ObjectType>().Max());
		var undefined = (ObjectType)(highestDefined + 1);

		_ = Assert.Throws<ArgumentOutOfRangeException>(() => ObjectTypeMapping.ObjectTypeToStructType(undefined));
	}
}
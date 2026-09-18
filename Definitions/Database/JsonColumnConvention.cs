using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Definitions.Database;

/// <summary>
/// Applies JSON storage to every complex property on the object sub-tables.
/// <para>
/// A property is considered complex (and therefore stored as JSON) when its CLR type is neither a
/// primitive/scalar type understood by EF Core nor an enum. Primitive collections (e.g.
/// <c>List&lt;uint8_t&gt;</c>) are already mapped to JSON by EF Core and only have their column type set.
/// </para>
/// No junction/pivot tables are created: object references are kept inline in the JSON payload.
/// </summary>
public static class JsonColumnConvention
{
	public const string ColumnType = "json";

	/// <summary>
	/// Registers the object-model building-block types as scalars (via a JSON value converter) so that EF Core
	/// does not discover them as accidental entity types / navigations. Collections of these types are then
	/// mapped as JSON primitive collections automatically.
	/// </summary>
	public static void Configure(ModelConfigurationBuilder configurationBuilder)
	{
		// Nested object-model classes.
		configurationBuilder.Properties<ObjectModelHeader>().HaveConversion<JsonValueConverter<ObjectModelHeader>>();
		configurationBuilder.Properties<Pos2>().HaveConversion<JsonValueConverter<Pos2>>();
		configurationBuilder.Properties<Pos3>().HaveConversion<JsonValueConverter<Pos3>>();
		configurationBuilder.Properties<BuildingComponents>().HaveConversion<JsonValueConverter<BuildingComponents>>();
		configurationBuilder.Properties<CargoOffset>().HaveConversion<JsonValueConverter<CargoOffset>>();
		configurationBuilder.Properties<AirportBuilding>().HaveConversion<JsonValueConverter<AirportBuilding>>();
		configurationBuilder.Properties<MovementNode>().HaveConversion<JsonValueConverter<MovementNode>>();
		configurationBuilder.Properties<MovementEdge>().HaveConversion<JsonValueConverter<MovementEdge>>();
		configurationBuilder.Properties<IndustryObjectRandomAnimation>().HaveConversion<JsonValueConverter<IndustryObjectRandomAnimation>>();
		configurationBuilder.Properties<IndustryObjectProductionRateRange>().HaveConversion<JsonValueConverter<IndustryObjectProductionRateRange>>();
		configurationBuilder.Properties<SteamImageAndHeight>().HaveConversion<JsonValueConverter<SteamImageAndHeight>>();
		configurationBuilder.Properties<MorphemeCategory>().HaveConversion<JsonValueConverter<MorphemeCategory>>();
		configurationBuilder.Properties<StringTableEntry>().HaveConversion<JsonValueConverter<StringTableEntry>>();
		configurationBuilder.Properties<SoundObjectData>().HaveConversion<JsonValueConverter<SoundObjectData>>();
		configurationBuilder.Properties<VehicleObjectCar>().HaveConversion<JsonValueConverter<VehicleObjectCar>>();
		configurationBuilder.Properties<BodySprite>().HaveConversion<JsonValueConverter<BodySprite>>();
		configurationBuilder.Properties<BogieSprite>().HaveConversion<JsonValueConverter<BogieSprite>>();
		configurationBuilder.Properties<EmitterAnimation>().HaveConversion<JsonValueConverter<EmitterAnimation>>();
		configurationBuilder.Properties<FrictionSound>().HaveConversion<JsonValueConverter<FrictionSound>>();
		configurationBuilder.Properties<SimpleMotorSound>().HaveConversion<JsonValueConverter<SimpleMotorSound>>();
		configurationBuilder.Properties<GearboxMotorSound>().HaveConversion<JsonValueConverter<GearboxMotorSound>>();

		// Collections / arrays / dictionaries (of the above or of primitives) are stored as a single JSON column.
		configurationBuilder.Properties<List<ObjectModelHeader>>().HaveConversion<JsonValueConverter<List<ObjectModelHeader>>>();
		configurationBuilder.Properties<List<AirportBuilding>>().HaveConversion<JsonValueConverter<List<AirportBuilding>>>();
		configurationBuilder.Properties<List<MovementNode>>().HaveConversion<JsonValueConverter<List<MovementNode>>>();
		configurationBuilder.Properties<List<MovementEdge>>().HaveConversion<JsonValueConverter<List<MovementEdge>>>();
		configurationBuilder.Properties<List<SteamImageAndHeight>>().HaveConversion<JsonValueConverter<List<SteamImageAndHeight>>>();
		configurationBuilder.Properties<List<IndustryObjectRandomAnimation>>().HaveConversion<JsonValueConverter<List<IndustryObjectRandomAnimation>>>();
		configurationBuilder.Properties<List<IndustryObjectProductionRateRange>>().HaveConversion<JsonValueConverter<List<IndustryObjectProductionRateRange>>>();
		configurationBuilder.Properties<List<MorphemeCategory>>().HaveConversion<JsonValueConverter<List<MorphemeCategory>>>();
		configurationBuilder.Properties<List<VehicleObjectCar>>().HaveConversion<JsonValueConverter<List<VehicleObjectCar>>>();
		configurationBuilder.Properties<List<BodySprite>>().HaveConversion<JsonValueConverter<List<BodySprite>>>();
		configurationBuilder.Properties<List<BogieSprite>>().HaveConversion<JsonValueConverter<List<BogieSprite>>>();
		configurationBuilder.Properties<List<EmitterAnimation>>().HaveConversion<JsonValueConverter<List<EmitterAnimation>>>();
		configurationBuilder.Properties<List<List<uint8_t>>>().HaveConversion<JsonValueConverter<List<List<uint8_t>>>>();
		configurationBuilder.Properties<List<uint8_t[]>>().HaveConversion<JsonValueConverter<List<uint8_t[]>>>();
		configurationBuilder.Properties<List<CargoCategory>[]>().HaveConversion<JsonValueConverter<List<CargoCategory>[]>>();
		configurationBuilder.Properties<Dictionary<CargoCategory, uint8_t>>().HaveConversion<JsonValueConverter<Dictionary<CargoCategory, uint8_t>>>();
		configurationBuilder.Properties<CargoOffset[][][]>().HaveConversion<JsonValueConverter<CargoOffset[][][]>>();
		configurationBuilder.Properties<uint8_t[][]>().HaveConversion<JsonValueConverter<uint8_t[][]>>();
	}

	public static void Apply(ModelBuilder modelBuilder)
	{
		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			if (!typeof(DbSubObject).IsAssignableFrom(entityType.ClrType))
			{
				continue;
			}

			foreach (var property in entityType.GetProperties())
			{
				// EF Core already maps primitive collections to a single JSON column.
				if (property.IsPrimitiveCollection)
				{
					property.SetColumnType(ColumnType);
					continue;
				}

				if (!IsComplexType(property.ClrType))
				{
					continue;
				}

				var converterType = typeof(JsonValueConverter<>).MakeGenericType(property.ClrType);
				property.SetValueConverter((ValueConverter)Activator.CreateInstance(converterType)!);

				var comparerType = typeof(JsonValueComparer<>).MakeGenericType(property.ClrType);
				property.SetValueComparer((ValueComparer)Activator.CreateInstance(comparerType)!);

				property.SetColumnType(ColumnType);
			}
		}
	}

	/// <summary>
	/// Returns <see langword="true"/> for types that EF Core cannot map as a scalar and that should therefore be
	/// persisted as JSON (lists, arrays, dictionaries and nested object-model classes).
	/// </summary>
	public static bool IsComplexType(Type type)
	{
		if (type.IsEnum)
		{
			return false;
		}

		var underlying = Nullable.GetUnderlyingType(type);
		if (underlying != null)
		{
			return IsComplexType(underlying);
		}

		if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal)
			|| type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly)
			|| type == typeof(TimeOnly) || type == typeof(TimeSpan) || type == typeof(Guid))
		{
			return false;
		}

		// byte[] is stored as a BLOB by EF Core, everything else (other arrays, collections, nested objects) as JSON.
		return type != typeof(byte[]);
	}
}

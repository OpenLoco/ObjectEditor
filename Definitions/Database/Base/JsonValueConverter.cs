using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Definitions.Database;

/// <summary>
/// Serialises a complex object-model property (list, nested object, dictionary, jagged array, ...)
/// to a JSON string so it can be persisted in a single SQLite <c>json</c> column.
/// </summary>
/// <typeparam name="T">The complex CLR type of the property.</typeparam>
public class JsonValueConverter<T> : ValueConverter<T, string>
	where T : notnull
{
	public static readonly JsonSerializerOptions SerializerOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		WriteIndented = false,
	};

	public JsonValueConverter()
		: base(
			value => Serialize(value),
			value => Deserialize(value))
	{ }

	public static string Serialize(T value)
		=> value is null ? string.Empty : JsonSerializer.Serialize(value, SerializerOptions);

	public static T Deserialize(string value)
		=> string.IsNullOrEmpty(value)
			? CreateDefault()
			: JsonSerializer.Deserialize<T>(value, SerializerOptions)!;

	/// <summary>
	/// Creates an empty instance for a value that was stored as an empty string (the default used by the
	/// SQLite provider when adding non-nullable columns to an existing table), so pre-existing rows read back as
	/// empty collections/objects rather than <see langword="null"/>.
	/// </summary>
	private static T CreateDefault()
	{
		var type = typeof(T);

		if (type == typeof(string))
		{
			return (T)(object)string.Empty;
		}

		if (type.IsArray)
		{
			return (T)(object)Array.CreateInstance(type.GetElementType()!, 0);
		}

		try
		{
			return (T?)Activator.CreateInstance(type) ?? default!;
		}
		catch (MissingMethodException)
		{
			return default!;
		}
	}
}

/// <summary>
/// Change-tracking comparer used for properties persisted through <see cref="JsonValueConverter{T}"/>.
/// Compares (and snapshots) the values by their JSON representation so collection/nested mutations are detected.
/// </summary>
/// <typeparam name="T">The complex CLR type of the property.</typeparam>
public class JsonValueComparer<T> : ValueComparer<T>
	where T : notnull
{
	public JsonValueComparer()
		: base(
			(a, b) => JsonValueConverter<T>.Serialize(a!) == JsonValueConverter<T>.Serialize(b!),
			v => v == null ? 0 : JsonValueConverter<T>.Serialize(v).GetHashCode(StringComparison.Ordinal),
			v => JsonValueConverter<T>.Deserialize(JsonValueConverter<T>.Serialize(v)))
	{ }
}

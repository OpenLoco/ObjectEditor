using System.Collections.Concurrent;
using System.Reflection;
using Definitions.Database;

namespace Definitions.DTO.Mappers;

/// <summary>
/// Bridges a sub-object DTO (e.g. <see cref="DtoObjectAirport"/>) to the table entity it represents
/// (e.g. <see cref="TblObjectAirport"/>) via the per-type <c>ToTbl...Entity</c> mappers.
/// <para>
/// The lookup is convention-based, so adding a new object type only means adding its mapper — there is
/// no hand-maintained switch to keep in sync with the object model. A missing or ambiguous mapper
/// throws rather than silently dropping sub-object data.
/// </para>
/// </summary>
public static class SubObjectDtoMapper
{
	private static readonly ConcurrentDictionary<Type, MethodInfo> MapperCache = new();

	/// <summary>
	/// Converts <paramref name="dto"/> into the sub-object table entity it represents, parented to
	/// <paramref name="parent"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">No unique reverse mapper exists for the DTO type.</exception>
	public static IDbSubObject ToTableEntity(IDtoSubObject dto, TblObject parent)
	{
		ArgumentNullException.ThrowIfNull(dto);
		ArgumentNullException.ThrowIfNull(parent);

		var mapper = MapperCache.GetOrAdd(dto.GetType(), ResolveMapper);
		return (IDbSubObject)mapper.Invoke(null, [dto, parent])!;
	}

	/// <summary>Returns the reverse mapper for <paramref name="dtoType"/>, or <see langword="null"/> when there is none.</summary>
	public static MethodInfo? FindMapperOrNull(Type dtoType)
	{
		var matches = FindMappers(dtoType);
		return matches.Count == 1 ? matches[0] : null;
	}

	static MethodInfo ResolveMapper(Type dtoType)
	{
		var matches = FindMappers(dtoType);

		return matches.Count switch
		{
			1 => matches[0],
			0 => throw new InvalidOperationException(
				$"No sub-object table mapper found for {dtoType.Name}. Add a 'ToTbl...Entity' mapper taking ({dtoType.Name}, TblObject)."),
			_ => throw new InvalidOperationException(
				$"Multiple sub-object table mappers found for {dtoType.Name}: {string.Join(", ", matches.Select(m => $"{m.DeclaringType!.Name}.{m.Name}"))}"),
		};
	}

	static List<MethodInfo> FindMappers(Type dtoType)
		=> [.. typeof(IDtoSubObject).Assembly
			.GetTypes()
			.Where(t => t.IsAbstract && t.IsSealed) // static classes
			.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
			.Where(m => typeof(IDbSubObject).IsAssignableFrom(m.ReturnType)
				&& m.GetParameters() is { Length: 2 } parameters
				&& parameters[0].ParameterType == dtoType
				&& parameters[1].ParameterType == typeof(TblObject))];
}
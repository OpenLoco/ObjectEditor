using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace Dat.FileParsing;

/// <summary>
/// Cached, per-type reflection metadata for a single loco-serialisable property (i.e. one carrying a
/// <see cref="LocoStructOffsetAttribute"/>). Property lookups and attribute scans are comparatively
/// expensive and were previously repeated for every struct instance (and every array element), so
/// they are computed once per type here and reused by <see cref="ByteReader"/> and
/// <see cref="ByteWriter"/>.
/// </summary>
internal sealed class LocoPropertyMetadata
{
	public required PropertyInfo Property { get; init; }

	public required Type PropertyType { get; init; }

	public required int Offset { get; init; }

	public bool IsArray { get; init; }

	/// <summary>Declared array length (from <see cref="LocoArrayLengthAttribute"/>), or 0 when the property is not an array.</summary>
	public int ArrayLength { get; init; }

	public Type? ElementType { get; init; }

	public bool IsVariableLoad { get; init; }

	public bool IsSkipRead { get; init; }
}

/// <summary>Process-wide cache of <see cref="LocoPropertyMetadata"/> arrays, keyed by struct type.</summary>
internal static class LocoStructMetadata
{
	static readonly ConcurrentDictionary<Type, LocoPropertyMetadata[]> StructCache = new();

	/// <summary>Returns the loco-serialisable properties of <paramref name="t"/> in declaration order.</summary>
	public static LocoPropertyMetadata[] GetStructProperties(Type t)
		=> StructCache.GetOrAdd(t, BuildStructProperties);

	static LocoPropertyMetadata[] BuildStructProperties(Type t)
	{
		var list = new List<LocoPropertyMetadata>();

		foreach (var p in t.GetProperties())
		{
			// ignore non-loco properties on the records
			var offsetAttr = AttributeHelper.Get<LocoStructOffsetAttribute>(p);
			if (offsetAttr == null)
			{
				continue;
			}

			var propertyType = p.PropertyType;
			var isArray = propertyType.IsArray;
			var elementType = isArray ? propertyType.GetElementType() : null;

			var skip = AttributeHelper.Has<LocoStructSkipReadAttribute>(p);
			var arrayLength = 0;
			if (isArray)
			{
				var arrLengthAttr = AttributeHelper.Get<LocoArrayLengthAttribute>(p);

				// The reader validated the attribute for every non-skipped array; preserve that.
				if (!skip && arrLengthAttr == null)
				{
					throw new ArgumentOutOfRangeException(nameof(t), $"type {t} with property {p} didn't have LocoArrayLength attribute specified");
				}

				arrayLength = arrLengthAttr?.Length ?? 0;
			}

			list.Add(new LocoPropertyMetadata
			{
				Property = p,
				PropertyType = propertyType,
				Offset = offsetAttr.Offset,
				IsArray = isArray,
				ArrayLength = arrayLength,
				ElementType = elementType,
				IsVariableLoad = AttributeHelper.Has<LocoStructVariableLoadAttribute>(p),
				IsSkipRead = skip,
			});
		}

		return [.. list];
	}
}

/// <summary>Cached flag/enum value metadata used to decode <see cref="FlagsAttribute"/> enums without
/// re-parsing every member on each read.</summary>
internal sealed class LocoEnumMetadata
{
	public bool IsFlags { get; init; }

	public bool IsSigned { get; init; }

	public int[] SignedValues { get; init; } = [];

	public uint[] UnsignedValues { get; init; } = [];
}

internal static class LocoEnumMetadataCache
{
	static readonly ConcurrentDictionary<Type, LocoEnumMetadata> EnumCache = new();

	public static LocoEnumMetadata Get(Type t)
		=> EnumCache.GetOrAdd(t, BuildEnumMetadata);

	static LocoEnumMetadata BuildEnumMetadata(Type t)
	{
		if (!t.IsDefined(typeof(FlagsAttribute), inherit: false))
		{
			return new LocoEnumMetadata { IsFlags = false };
		}

		var underlyingType = t.GetEnumUnderlyingType();
		var values = Enum.GetValues(t);

		// Mirror the loco type aliases (uint8_t = byte, int16_t = short, ...); anything else is unsupported.
		if (underlyingType == typeof(sbyte) || underlyingType == typeof(short) || underlyingType == typeof(int))
		{
			var signed = new int[values.Length];
			for (var i = 0; i < values.Length; i++)
			{
				signed[i] = Convert.ToInt32(values.GetValue(i), CultureInfo.InvariantCulture);
			}

			return new LocoEnumMetadata { IsFlags = true, IsSigned = true, SignedValues = signed };
		}

		if (underlyingType == typeof(byte) || underlyingType == typeof(ushort) || underlyingType == typeof(uint))
		{
			var unsigned = new uint[values.Length];
			for (var i = 0; i < values.Length; i++)
			{
				unsigned[i] = Convert.ToUInt32(values.GetValue(i), CultureInfo.InvariantCulture);
			}

			return new LocoEnumMetadata { IsFlags = true, IsSigned = false, UnsignedValues = unsigned };
		}

		throw new ArgumentOutOfRangeException(nameof(t), underlyingType, "unrecognised type");
	}
}

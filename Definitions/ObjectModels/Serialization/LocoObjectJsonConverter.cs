using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Serialization
{
	public sealed class LocoObjectJsonConverter : JsonConverter<LocoObject>
	{
		private const string ObjectTypeProp = nameof(LocoObject.ObjectType);
		private const string ObjectProp = nameof(LocoObject.Object);
		private const string StringTableProp = nameof(LocoObject.StringTable);
		private const string ImageTableProp = nameof(LocoObject.ImageTable);
		private const string ObjectClrTypeProp = "ObjectClrType";

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "LocoObjectJsonConverter uses reflection-based JSON serialization and Type.GetType() for dynamic type resolution. Callers are responsible for ensuring required types are preserved.")]
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2057", Justification = "The type name stored in JSON is always a valid assembly-qualified type name written by this same converter.")]
		public override LocoObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using var doc = JsonDocument.ParseValue(ref reader);
			var root = doc.RootElement;

			// Read required fields
			if (!root.TryGetProperty(ObjectTypeProp, out var objTypeEl))
			{
				throw new JsonException($"Missing required property '{ObjectTypeProp}'.");
			}

			if (!root.TryGetProperty(ObjectProp, out var objEl))
			{
				throw new JsonException($"Missing required property '{ObjectProp}'.");
			}

			if (!root.TryGetProperty(StringTableProp, out var stringTableEl))
			{
				throw new JsonException($"Missing required property '{StringTableProp}'.");
			}

			// Determine the concrete ILocoStruct type
			if (!root.TryGetProperty(ObjectClrTypeProp, out var clrTypeEl))
			{
				throw new JsonException($"Missing required property '{ObjectClrTypeProp}' needed to deserialize '{ObjectProp}'.");
			}

			var objectType = objTypeEl.Deserialize<ObjectType>(options);
			var stringTable = stringTableEl.Deserialize<StringTable>(options)
				?? throw new JsonException($"Could not deserialize '{StringTableProp}'.");

			ImageTable? imageTable = null;
			if (root.TryGetProperty(ImageTableProp, out var imageTableEl) && imageTableEl.ValueKind != JsonValueKind.Null)
			{
				imageTable = imageTableEl.Deserialize<ImageTable>(options);
			}

			var clrTypeName = clrTypeEl.GetString();
			if (string.IsNullOrWhiteSpace(clrTypeName))
			{
				throw new JsonException($"'{ObjectClrTypeProp}' was null or empty.");
			}

			var concreteType = Type.GetType(clrTypeName, throwOnError: true)
				?? throw new JsonException($"Could not resolve type '{clrTypeName}'.");

			var locoStruct = (ILocoStruct?)objEl.Deserialize(concreteType, options)
				?? throw new JsonException($"Could not deserialize '{ObjectProp}' as '{concreteType}'.");

			return new LocoObject(objectType, locoStruct, stringTable, imageTable);
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "LocoObjectJsonConverter uses reflection-based JSON serialization and runtime type information. Callers are responsible for ensuring required types are preserved.")]
		public override void Write(Utf8JsonWriter writer, LocoObject value, JsonSerializerOptions options)
		{
			if (value is null)
			{
				writer.WriteNullValue();
				return;
			}

			if (value.Object is null)
			{
				throw new JsonException("LocoObject.Object was null during serialization.");
			}

			writer.WriteStartObject();

			// ObjectType
			writer.WritePropertyName(ObjectTypeProp);
			JsonSerializer.Serialize(writer, value.ObjectType, options);

			// Underlying CLR type of the ILocoStruct (prints the correct underlying type)
			writer.WriteString(ObjectClrTypeProp, value.Object.GetType().AssemblyQualifiedName);

			// Object (serialize using concrete runtime type)
			writer.WritePropertyName(ObjectProp);
			JsonSerializer.Serialize(writer, value.Object, value.Object.GetType(), options);

			// StringTable
			writer.WritePropertyName(StringTableProp);
			JsonSerializer.Serialize(writer, value.StringTable, options);

			// ImageTable (may be null)
			writer.WritePropertyName(ImageTableProp);
			JsonSerializer.Serialize(writer, value.ImageTable, options);

			writer.WriteEndObject();
		}
	}
}


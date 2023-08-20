using OpenLocoTool.Objects;

namespace OpenLocoTool.DatFileParsing
{
	public static class StaticByteReader
	{
		public static uint8_t Read_uint8t(ReadOnlySpan<byte> data, int offset)
			=> data[offset];

		public static int8_t Read_int8t(ReadOnlySpan<byte> data, int offset)
			=> (sbyte)data[offset];

		public static uint16_t Read_uint16t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToUInt16(data[offset..(offset + 2)]);

		public static int16_t Read_int16t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToInt16(data[offset..(offset + 2)]);

		public static uint32_t Read_uint32t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToUInt32(data[offset..(offset + 4)]);

		public static int32_t Read_int32t(ReadOnlySpan<byte> data, int offset)
			=> BitConverter.ToInt32(data[offset..(offset + 4)]);

		// this method isn't necessary normally since you can just call the above methods ^
		// but it saves a lot of code rewriting for the Read_Array method
		public static T Read<T>(ReadOnlySpan<byte> data, int offset) where T : struct
		{
			var t = typeof(T);
			if (t == typeof(uint8_t))
			{
				return (dynamic)Read_uint8t(data, offset);
			}

			throw new NotImplementedException("");
		}

		public static T[] Read_Array<T>(ReadOnlySpan<byte> data, int offset, int count) where T : struct
		{
			var arr = new T[count];
			for (var i = 0; i < count; i++)
			{
				arr[i] = Read<T>(data, offset);
			}
			return arr;
		}
	}

	public ref struct ByteReader
	{
		public ByteReader(ReadOnlySpan<byte> data)
			=> this.data = data;

		ReadOnlySpan<byte> data { get; }

		int ObjectSize(Type type)
		{
			var size = 0;
			if (type == typeof(byte) || type == typeof(sbyte))
			{
				size = 1;
			}
			else if (type == typeof(uint16_t) || type == typeof(int16_t))
			{
				size = 2;
			}
			else if (type == typeof(uint32_t) || type == typeof(int32_t))
			{
				size = 4;
			}
			else
			{
				var attrs = type.GetCustomAttributes(typeof(LocoStructSizeAttribute), inherit: false);
				if (attrs.Length != 1)
				{
					throw new ArgumentOutOfRangeException(nameof(LocoArrayLengthAttribute), $"type {type} didn't have LocoArrayLength attribute specified");
				}
				size = ((LocoStructSizeAttribute)attrs[0]).Size;
			}

			if (size == 0)
			{
				throw new ArgumentException("unknown primitive type with no size");
			}

			return size;
		}

		public object Read_ArrayT(Type t, int position, int length)
		{
			var elementType = t.GetElementType();
			var size = ObjectSize(elementType);

			var arr = Array.CreateInstance(elementType, length);
			for (var i = 0; i < length; i++)
			{
				arr.SetValue(ReadT(elementType, position + (i * size)), i);
			}
			return arr;
		}

		public object ReadT(Type t, int position, int arrLength = 0)
		{
			if (t == typeof(uint8_t))
			{
				return StaticByteReader.Read_uint8t(data, position);
			}
			if (t == typeof(int8_t))
			{
				return StaticByteReader.Read_int8t(data, position);
			}
			if (t == typeof(uint16_t))
			{
				return StaticByteReader.Read_uint16t(data, position);
			}
			if (t == typeof(int16_t))
			{
				return StaticByteReader.Read_int16t(data, position);
			}
			if (t == typeof(uint32_t))
			{
				return StaticByteReader.Read_uint32t(data, position);
			}
			if (t == typeof(int32_t))
			{
				return StaticByteReader.Read_int32t(data, position);
			}
			if (t == typeof(string_id))
			{
				return StaticByteReader.Read_uint16t(data, position);
			}
			if (t.IsArray)
			{
				return Read_ArrayT(t, position, arrLength);
			}
			if (t.IsEnum) // this is so big because we need special handling for 'flags' enums
			{
				var underlyingType = t.GetEnumUnderlyingType();
				var underlyingValue = ReadT(underlyingType, position); // Read the underlying value

				if (t.IsDefined(typeof(FlagsAttribute), inherit: false))
				{
					var enumValues = Enum.GetValues(t);
					var combinedValue = 0;

					foreach (var enumValue in enumValues)
					{
						var enumValueInt = Convert.ToInt32(Enum.Parse(t, enumValue.ToString())); // Convert to int
						if ((enumValueInt & Convert.ToInt32(underlyingValue)) != 0) // Convert to int
						{
							combinedValue |= enumValueInt;
						}
					}

					var enu = Enum.ToObject(t, combinedValue);
					return enu;
				}
				else
				{
					var enu = Enum.ToObject(t, underlyingValue);
					return enu;
				}
			}
			if (t.IsClass)
			{
				var objectSize = ObjectSize(t);
				return ReadLocoStruct(data[position..(position + objectSize)], t);
			}

			//if (t.IsValueType)
			//{
			//	var size = StructSizeLookup[t];
			//	return CastReadOnlySpanToStruct(t, data[StreamPosition..(StreamPosition + size)]);
			//}

			throw new NotImplementedException(t.ToString());
		}

		// todo: can just use attributes and reflection to avoid using this table at all
		static Dictionary<Type, int> StructSizeLookup = new()
		{
			// misc
			{ typeof(Pos2), Pos2.StructLength },
			{ typeof(BuildingPartAnimation), BuildingPartAnimation.StructLength },
			{ typeof(IndustryObjectUnk38), IndustryObjectUnk38.StructLength },
			{ typeof(IndustryObjectProductionRateRange), IndustryObjectProductionRateRange.StructLength },
			{ typeof(TownNamesUnk), TownNamesUnk.StructLength },
			{ typeof(ImageAndHeight), ImageAndHeight.StructLength },
			// vehicles
			{ typeof(BodySprite), BodySprite.StructLength },
			{ typeof(BogieSprite), BogieSprite.StructLength },
			{ typeof(Engine1Sound), Engine1Sound.StructLength },
			{ typeof(Engine2Sound), Engine2Sound.StructLength },
			{ typeof(FrictionSound), FrictionSound.StructLength },
			{ typeof(SimpleAnimation), SimpleAnimation.StructLength },
			{ typeof(VehicleObjectUnk), VehicleObjectUnk.StructLength },
		};

		public static object CastReadOnlySpanToStruct(Type structType, ReadOnlySpan<byte> span)
		{
			if (!structType.IsValueType)
			{
				throw new ArgumentException(nameof(structType));
			}

			var result = Activator.CreateInstance(structType);

			var properties = structType.GetProperties();
			for (var i = 0; i < properties.Length && i < span.Length; i++)
			{
				var property = properties[i];
				var locoAttrs = property.GetCustomAttributes(typeof(LocoStructOffsetAttribute), inherit: false);
				if (!locoAttrs.Any())
				{
					continue;
				}
				var propertyValue = Convert.ChangeType(span[i], property.PropertyType);
				property.SetValue(result, propertyValue);
			}

			return result;
		}

		public static ILocoStruct ReadLocoStruct<T>(ReadOnlySpan<byte> data) where T : class
			=> ReadLocoStruct(data, typeof(T));

		public static ILocoStruct ReadLocoStruct(ReadOnlySpan<byte> data, Type t)
		{
			var properties = t.GetProperties();
			var args = new List<object>();
			var byteReader = new ByteReader(data);

			foreach (var p in properties)
			{
				// ignore non-binary properties on the records
				var locoAttrs = p.GetCustomAttributes(typeof(LocoStructOffsetAttribute), inherit: false);
				if (!locoAttrs.Any())
				{
					continue;
				}
				var locoProperty = (LocoStructOffsetAttribute)locoAttrs[0];

				// special array handling
				if (p.PropertyType.IsArray)
				{
					var attrs = p.GetCustomAttributes(typeof(LocoArrayLengthAttribute), inherit: false);
					if (attrs.Length != 1)
					{
						throw new ArgumentOutOfRangeException(nameof(LocoArrayLengthAttribute), $"type {t} with property {p} didn't have LocoArrayLength attribute specified");
					}
					args.Add(byteReader.ReadT(p.PropertyType, locoProperty.Offset, ((LocoArrayLengthAttribute)attrs[0]).Length));
				}
				else
				{
					args.Add(byteReader.ReadT(p.PropertyType, locoProperty.Offset, 0));
				}
			}

			try
			{
				var argsArr = args.ToArray();
				if (argsArr.Length == 0)
				{
					throw new ArgumentException($"{nameof(argsArr)} had no arguments to construct an object");
				}

				var newInstance = (ILocoStruct)Activator.CreateInstance(t, argsArr);
				return newInstance == null
					? throw new InvalidDataException("couldn't parse")
					: newInstance;
			}
			catch (Exception ex)
			{
				return default;
			}
		}
	}
}

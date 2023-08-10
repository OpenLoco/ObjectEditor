using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
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

		public static int32_t Read_int32(ReadOnlySpan<byte> data, int offset)
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
		int StreamPosition { get; set; }

		public uint8_t Read_uint8t()
		{
			var tmp = data[StreamPosition];
			StreamPosition += 1;
			return tmp;
		}

		public int8_t Read_int8t()
		{
			var tmp = (sbyte)data[StreamPosition];
			StreamPosition += 1;
			return tmp;
		}

		public uint16_t Read_uint16t()
		{
			var tmp = BitConverter.ToUInt16(data[StreamPosition..(StreamPosition + 2)]);
			StreamPosition += 2;
			return tmp;
		}

		public int16_t Read_int16t()
		{
			var tmp = BitConverter.ToInt16(data[StreamPosition..(StreamPosition + 2)]);
			StreamPosition += 2;
			return tmp;
		}

		public uint32_t Read_uint32t()
		{
			var tmp = BitConverter.ToUInt32(data[StreamPosition..(StreamPosition + 4)]);
			StreamPosition += 4;
			return tmp;
		}

		public int32_t Read_int32t()
		{
			var tmp = BitConverter.ToInt32(data[StreamPosition..(StreamPosition + 4)]);
			StreamPosition += 4;
			return tmp;
		}

		public int32_t Read_stringid()
		{
			return Read_uint16t();
		}

		public object Read_ArrayT(Type t, int length)
		{
			var elementType = t.GetElementType() ?? throw new ArgumentException($"Unable to get array element type for type {t}");
			var arr = Array.CreateInstance(elementType, length);
			for (var i = 0; i < length; i++)
			{
				arr.SetValue(ReadT(elementType), i);
			}
			return arr;
		}

		public object ReadT(Type t, int position, int arrLength = 0)
		{
			StreamPosition = position;
			return ReadT(t, arrLength);
		}

		public object ReadT(Type t, int arrLength = 0)
		{
			if (t == typeof(uint8_t))
			{
				return Read_uint8t();
			}
			if (t == typeof(int8_t))
			{
				return Read_int8t();
			}
			if (t == typeof(uint16_t))
			{
				return Read_uint16t();
			}
			if (t == typeof(int16_t))
			{
				return Read_int16t();
			}
			if (t == typeof(uint32_t))
			{
				return Read_uint32t();
			}
			if (t == typeof(int32_t))
			{
				return Read_int32t();
			}
			if (t == typeof(string_id))
			{
				return Read_stringid();
			}
			if (t.IsArray)
			{
				return Read_ArrayT(t, arrLength);
			}
			if (t.IsEnum) // this is so big because we need special handling for 'flags' enums
			{
				var underlyingType = t.GetEnumUnderlyingType();
				var underlyingValue = ReadT(underlyingType); // Read the underlying value

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
				return ReadLocoStruct(data, t);
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
			{ typeof(Pos2), Pos2.ObjectStructSize },
			{ typeof(BuildingPartAnimation), BuildingPartAnimation.ObjectStructSize },
			{ typeof(IndustryObjectUnk38), IndustryObjectUnk38.ObjectStructSize },
			{ typeof(IndustryObjectProductionRateRange), IndustryObjectProductionRateRange.ObjectStructSize },
			{ typeof(TownNamesUnk), TownNamesUnk.ObjectStructSize },
			{ typeof(ImageAndHeight), ImageAndHeight.ObjectStructSize },
			// vehicles
			{ typeof(BodySprite), BodySprite.ObjectStructSize },
			{ typeof(BogieSprite), BogieSprite.ObjectStructSize },
			{ typeof(Engine1Sound), Engine1Sound.ObjectStructSize },
			{ typeof(Engine2Sound), Engine2Sound.ObjectStructSize },
			{ typeof(FrictionSound), FrictionSound.ObjectStructSize },
			{ typeof(SimpleAnimation), SimpleAnimation.ObjectStructSize },
			{ typeof(VehicleObjectUnk), VehicleObjectUnk.ObjectStructSize },
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
				var locoAttrs = property.GetCustomAttributes(typeof(LocoStructPropertyAttribute), inherit: false);
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
				var locoAttrs = p.GetCustomAttributes(typeof(LocoStructPropertyAttribute), inherit: false);
				if (!locoAttrs.Any())
				{
					continue;
				}
				var locoProperty = (LocoStructPropertyAttribute)locoAttrs[0];

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
					args.Add(byteReader.ReadT(p.PropertyType, locoProperty.Offset));
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

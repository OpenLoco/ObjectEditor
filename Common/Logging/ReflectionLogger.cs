using System.Reflection;
using System.Text;

namespace Common.Logging;

public static class ReflectionLogger
{
	public static string ToString<T>(T obj)
	{
		var sb = new StringBuilder();
		return ToString(obj, sb).ToString();
	}

	static StringBuilder ToString<T>(T obj, StringBuilder sb)
	{
		if (obj == null)
		{
			_ = sb.Append("<null>");
			return sb;
		}

		var type = obj.GetType();

		// Check if the type has a custom ToString method, in which case use it
		//if (type.GetMethod("ToString", Array.Empty<Type>()).DeclaringType != typeof(object))
		//{
		//	sb.Append(obj.ToString());
		//}

		// For primitive types, use their ToString representation directly
		if (type.IsPrimitive || obj is string)
		{
			_ = sb.Append(obj.ToString());
			return sb;
		}

		// For other reference types, recursively print their properties
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

		_ = sb.Append(type.Name);

		if (properties.Length > 0 || fields.Length > 0)
		{
			_ = sb.Append(" { ");

			for (var i = 0; i < fields.Length; i++)
			{
				var field = fields[i];
				var fieldName = field.Name;
				var fieldValue = field.GetValue(obj);

				_ = sb.Append(fieldName);
				_ = sb.Append('=');
				_ = sb.Append(ToString(fieldValue));

				if (i < fields.Length - 1)
				{
					_ = sb.Append(", ");
				}
			}

			for (var i = 0; i < properties.Length; i++)
			{
				var property = properties[i];
				var propertyName = property.Name;
				var propertyValue = property.GetValue(obj);

				_ = sb.Append(propertyName);
				_ = sb.Append('=');
				_ = sb.Append(ToString(propertyValue));

				if (i < properties.Length - 1)
				{
					_ = sb.Append(", ");
				}
			}

			_ = sb.Append(" } ");
		}

		return sb;
	}
}

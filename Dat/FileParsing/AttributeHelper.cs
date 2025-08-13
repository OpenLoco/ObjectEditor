using System.Reflection;

namespace Dat.FileParsing;

public static class AttributeHelper
{
	//public static T? Get<T>(Type t) where T : Attribute
	//{
	//	var attributes = t.GetCustomAttributes(typeof(T), inherit: false);
	//	return attributes.Length == 1 ? attributes[0] as T : null;
	//}

	public static T? Get<T>(PropertyInfo p) where T : Attribute
	{
		var attributes = p.GetCustomAttributes(typeof(T), inherit: false);
		return attributes.Length == 1 ? attributes[0] as T : null;
	}

	public static T? Get<T>(Type t) where T : Attribute
		=> t.GetCustomAttribute<T>(inherit: false);

	public static bool Has<T>(PropertyInfo p) where T : Attribute
		=> p.GetCustomAttribute<T>(inherit: false) is not null;

	public static bool Has<T>(Type t) where T : Attribute
		=> t.GetCustomAttribute<T>(inherit: false) is not null;

	public static IEnumerable<PropertyInfo> GetAllPropertiesWithAttribute<T>(Type t) where T : Attribute
		=> t.GetProperties().Where(Has<T>);
}

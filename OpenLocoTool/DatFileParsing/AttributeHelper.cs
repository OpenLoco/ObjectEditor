using System.Reflection;
using OpenLocoTool.Headers;

namespace OpenLocoTool.DatFileParsing
{
	public static class AttributeHelper
	{
		public static T? Get<T>(PropertyInfo p) where T : Attribute
		{
			var attrs = p.GetCustomAttributes(typeof(T), inherit: false);
			return attrs.Length == 1 ? attrs[0] as T : null;
		}

		public static T? Get<T>(Type t) where T : Attribute
			=> t.GetCustomAttribute(typeof(T), inherit: false) as T;

		public static bool Has<T>(PropertyInfo p) where T : Attribute
			=> p.GetCustomAttribute(typeof(T), inherit: false) is T;

		public static IEnumerable<PropertyInfo> GetAllPropertiesWithAttribute<T>(Type t) where T : Attribute
			=> t.GetProperties().Where(Has<T>);
	}

	public static class ObjectAttributes
	{
		public static int StructSize<T>() where T : ILocoStruct
			=> AttributeHelper.Get<LocoStructSizeAttribute>(typeof(T)).Size;

		public static ObjectType ObjectType<T>() // where T : ILocoStruct
			=> AttributeHelper.Get<LocoStructTypeAttribute>(typeof(T)).ObjectType;

		public static string[] StringTableNames<T>() // where T : ILocoStruct
			=> AttributeHelper.Get<LocoStringTableAttribute>(typeof(T)).Strings;
	}
}

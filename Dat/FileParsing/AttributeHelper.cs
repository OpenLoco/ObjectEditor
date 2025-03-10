using OpenLoco.Dat.Data;
using OpenLoco.Dat.Types;
using System.Reflection;

namespace OpenLoco.Dat.FileParsing
{
	public static class AttributeHelper
	{
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

	public static class ObjectAttributes
	{
		public static int StructSize<T>() where T : ILocoStruct
			=> AttributeHelper.Get<LocoStructSizeAttribute>(typeof(T))!.Size;

		public static ObjectType ObjectType<T>() // where T : ILocoStruct
			=> AttributeHelper.Get<LocoStructTypeAttribute>(typeof(T))!.ObjectType;

		public static ObjectType ObjectType(ILocoStruct str) // where T : ILocoStruct
			=> AttributeHelper.Get<LocoStructTypeAttribute>(str.GetType())!.ObjectType;

		public static string[] StringTableNames<T>() // where T : ILocoStruct
			=> AttributeHelper.Get<LocoStringTableAttribute>(typeof(T))!.Strings;
	}
}

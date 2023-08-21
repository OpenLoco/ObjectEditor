using System.Reflection;

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
	}
}

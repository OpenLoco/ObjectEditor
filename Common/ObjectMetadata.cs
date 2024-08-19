using System.ComponentModel;

namespace OpenLoco.Common
{
	public class ObjectMetadata
	{
		public ObjectMetadata(string originalName, uint originalChecksum)
		{
			OriginalName = originalName;
			OriginalChecksum = originalChecksum;
		}

		public string OriginalName { get; }
		public uint OriginalChecksum { get; }
		public string Description { get; set; }
		public string Author { get; set; } = "<unknown>";
		public string Version { get; set; } // author-specified version
		public DateTimeOffset CreatedTime { get; set; } // creation UTC date is an implicit version
		public DateTimeOffset LastEditTime { get; set; } // last-edited UTC date is an implicit version

		[Browsable(false)]
		public List<string> Tags { get; set; } = [];
	}
}

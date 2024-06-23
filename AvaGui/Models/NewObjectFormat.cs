using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaGui.Models
{
	// no encoding - just zip the whole object if necessary
	// no checksum - its now a json object, not binary
	public class NewObjectFormat<T>
	{
		public string Name { get; set; }
		public string Author { get; set; } // new
		public string Version { get; set; } // author-specified version
		public DateTimeOffset LastEdit { get; set; } // last-edited UTC date is an implicit version

		public SourceGame SourceGame { get; set; }
		public ObjectType ObjectType { get; set; }

		// obect properties
		public T Object { get; set; }

		public StringTable StringTable { get; set; }
		public NewImageTable ImageTable { get; set; }
	}

	public class NewImageTable
	{
		public IEnumerable<G1Element32> Elements { get; set; }
		public IEnumerable<byte[]> ElementData { get; set; } // this will actually be base-64 encoded strings, not binary data
	}
}

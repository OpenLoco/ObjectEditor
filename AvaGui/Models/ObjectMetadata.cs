using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AvaGui.Models
{
	// no encoding - just zip the whole object if necessary
	// no checksum - its now a json object, not binary
	public class ObjectMetadata
	{
		public ObjectMetadata(string originalName, uint originalChecksum)
		{
			OriginalName = originalName;
			OriginalChecksum = originalChecksum;
		}

		public string OriginalName { get; }
		public uint OriginalChecksum { get; }
		public string Author { get; set; } = "<unknown>";
		public string Version { get; set; } // author-specified version
		public DateTimeOffset CreatedTime { get; set; } // creation UTC date is an implicit version
		public DateTimeOffset LastEditTime { get; set; } // last-edited UTC date is an implicit version

		[Browsable(false)]
		public ObservableCollection<string> Tags { get; set; } = [];

		// object properties
		//public SourceGame SourceGame { get; set; }

		//public ObjectType ObjectType { get; set; }

		//public ILocoStruct Object { get; set; }

		//public StringTable StringTable { get; set; }
		//public NewImageTable ImageTable { get; set; }
	}

	//public class NewImageTable
	//{
	//	public IEnumerable<G1Element32> Elements { get; set; }
	//	public IEnumerable<byte[]> ElementData { get; set; } // this will actually be base-64 encoded strings, not binary data
	//}
}

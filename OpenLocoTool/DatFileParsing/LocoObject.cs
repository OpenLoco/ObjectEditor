﻿using System.ComponentModel;
using OpenLocoTool.Headers;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DatFileInfo(S5Header s5Header, ObjectHeader objectHeader)
	{
		public S5Header S5Header { get; set; } = s5Header;
		public ObjectHeader ObjectHeader { get; set; } = objectHeader;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject : ILocoObject
	{
		public LocoObject(S5Header s5Hdr, ObjectHeader objHdr, ILocoStruct obj, StringTable stringTable, List<G1Element32>? g1Elements)
		{
			S5Header = s5Hdr;
			ObjectHeader = objHdr;
			Object = obj;
			StringTable = stringTable;
			G1Elements = g1Elements;
		}
		public LocoObject(S5Header s5Hdr, ObjectHeader objHdr, ILocoStruct obj, StringTable stringTable)
		{
			S5Header = s5Hdr;
			ObjectHeader = objHdr;
			Object = obj;
			StringTable = stringTable;
			G1Elements = null;
		}

		public S5Header S5Header { get; set; }
		public ObjectHeader ObjectHeader { get; set; }
		public ILocoStruct Object { get; set; }
		public StringTable StringTable { get; set; }
		public List<G1Element32>? G1Elements { get; set; }
	}
}

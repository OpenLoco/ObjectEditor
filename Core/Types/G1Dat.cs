using System.ComponentModel;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class G1Dat(G1Header g1Header, List<G1Element32> g1Elements)
	{
		public G1Header G1Header { get; set; } = g1Header;
		public List<G1Element32> G1Elements { get; set; } = g1Elements;
	}
}

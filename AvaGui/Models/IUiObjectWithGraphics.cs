using OpenLoco.Dat.Types;
using System.Collections.Generic;
using System.ComponentModel;

namespace AvaGui.Models
{
	[TypeConverter(typeof(TypeListConverter))]
	public interface IUiObjectWithGraphics
	{
		public List<G1Element32> G1Elements { get; set; }
	}
}

using OpenLoco.ObjectEditor.Headers;
using System.Collections.Generic;

namespace AvaGui.Models
{
	public interface IUiObjectWithGraphics
	{
		public List<G1Element32> G1Elements { get; set; }
	}
}

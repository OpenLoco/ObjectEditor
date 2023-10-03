using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLocoTool
{
	public class Annotation
	{
		private string name = "";
		private Annotation? parent = null;
		private int start = 0;
		private int end = 0;
		private int length = 0;

		public Annotation(string name, Annotation? parent, int start, int length)
		{
			Name = name;
			Parent = parent;
			this.start = start;
			Length = length;
		}

		public Annotation(string name, int start, int length)
		{
			Name = name;
			this.start = start;
			Length = length;
		}

		public string Name { get => name; set => name = value; }
		public Annotation? Parent { get => parent; set => parent = value; }
		public int Start { get => start; set { start = value; Length = Length; } }
		public int End { get => end; set { end = value; length = end - start; } }
		public int Length { get => length; set { length = value; end = start + length; } }
	}
}

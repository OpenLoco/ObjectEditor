namespace OpenLoco.ObjectEditor
{
	public class Annotation
	{
		private int start;
		private int end;
		private int length;

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

		public string Name { get; set; }
		public Annotation? Parent { get; set; }

		public int Start
		{
			get => start;
			set
			{
				start = value;
				Length = length;
			}
		}

		public int End
		{
			get => end;
			set
			{
				end = value;
				length = end - start;
			}
		}

		public int Length
		{
			get => length;
			set
			{
				length = value;
				end = start + length;
			}
		}
	}
}

namespace OpenLoco.Dat
{
	public class HexAnnotation
	{
		private int start;
		private int end;
		private int length;

		public HexAnnotation(string name, HexAnnotation? parent, int start, int length)
		{
			Name = name;
			Parent = parent;
			this.start = start;
			Length = length;
		}

		public HexAnnotation(string name, int start, int length)
		{
			Name = name;
			this.start = start;
			Length = length;
		}

		public string Name { get; set; }
		public HexAnnotation? Parent { get; set; }

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

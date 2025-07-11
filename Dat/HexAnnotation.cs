namespace Dat;

public class HexAnnotation
{
	int start;
	int end;
	int length;

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

	public string OffsetText
		=> string.Format("(0x{0:X}-0x{1:X})", Start, End);

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

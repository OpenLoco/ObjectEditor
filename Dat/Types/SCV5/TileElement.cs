using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	public enum ElementType : uint8_t
	{
		Surface,
		Track,
		Station,
		Signal,
		Building,
		Tree,
		Wall,
		Road,
		Industry,
	};

	[LocoStructSize(StructLength)]
	public class TileElement
	{
		public const int StructLength = 0x08;

		public const uint8_t FLAG_GHOST = 1 << 4;
		public const uint8_t FLAG_LAST = 1 << 7;

		public ElementType Type { get; set; }
		public uint8_t Flags { get; set; }
		public uint8_t BaseZ { get; set; }
		public uint8_t ClearZ { get; set; }
		[LocoArrayLength(4)]
		public uint8_t[] var_4 { get; set; } // these vary per element type

		void SetLast(bool value)
		{
			if (value)
			{
				Flags |= FLAG_LAST;
			}
			else
			{
				unchecked
				{
					Flags &= (byte)~FLAG_LAST;
				}
			}
		}

		bool IsGhost() => (Flags & FLAG_GHOST) == FLAG_GHOST;

		public bool IsLast() => (Flags & FLAG_LAST) == FLAG_LAST;

		public bool IsWater() => (var_4[1] & 0x1F) != 0;

		public static TileElement Read(ReadOnlySpan<byte> data)
		{
			ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, StructLength);
			return new TileElement
			{
				Type = (ElementType)(data[0] & (0x3C >> 2)), // https://github.com/OpenLoco/OpenLoco/blob/master/src/OpenLoco/src/Map/Tile.cpp#L23
				Flags = data[1],
				BaseZ = data[2],
				ClearZ = data[3],
				var_4 = data[4..8].ToArray()
			};
		}
	}
}

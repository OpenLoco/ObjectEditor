using OpenLoco.Dat.FileParsing;

namespace Core.Types.SCV5
{
	[LocoStructSize(0x08)]
	public class TileElement
	{
		public const uint8_t FLAG_GHOST = 1 << 4;
		public const uint8_t FLAG_LAST = 1 << 7;

		public uint8_t Type { get; set; }
		public uint8_t Flags { get; set; }
		public uint8_t BaseZ { get; set; }
		public uint8_t ClearZ { get; set; }
		[LocoArrayLength(4)]
		public uint8_t[] pad_4 { get; set; }

		void SetLast(bool value)
		{
			if (value)
				Flags |= FLAG_LAST;
			else
			{
				unchecked
				{
					Flags &= (byte)~FLAG_LAST;
				}
			}
		}

		bool IsGhost()
		{
			return (Flags & FLAG_GHOST) == FLAG_GHOST;
		}

		bool IsLast()
		{
			return (Flags & FLAG_LAST) == FLAG_LAST;
		}
	}
}

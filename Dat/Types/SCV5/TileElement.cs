using Dat.FileParsing;

namespace Dat.Types.SCV5
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
	}

	[LocoStructSize(StructLength)]
	public abstract class TileElement
	{
		public const int StructLength = 0x08;

		public const uint8_t FLAG_GHOST = 1 << 4;
		public const uint8_t FLAG_LAST = 1 << 7;

		public ElementType Type { get; set; }
		public uint8_t Flags { get; set; }
		public uint8_t BaseZ { get; set; }
		public uint8_t ClearZ { get; set; }

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

		public static TileElement Read(ReadOnlySpan<byte> data)
		{
			ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, StructLength);

			var Type = (ElementType)((data[0] & 0x3C) >> 2); // https://github.com/OpenLoco/OpenLoco/blob/master/src/OpenLoco/src/Map/Tile.cpp#L23

			return Type switch
			{
				ElementType.Building => new BuildingElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = BitConverter.ToUInt16(data[6..8]) },
				ElementType.Industry => new IndustryElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], IndustryId = data[4], _5 = data[5], _6 = BitConverter.ToUInt16(data[6..8]) },
				ElementType.Road => new RoadElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
				ElementType.Signal => new SignalElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], LeftSide = new SignalElement.Side() { _4 = data[4], _5 = data[5] }, RightSide = new SignalElement.Side() { _4 = data[6], _5 = data[7] } },
				ElementType.Station => new StationElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], StationId = BitConverter.ToUInt16(data[6..8]) },
				ElementType.Surface => new SurfaceElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], Slope = data[4], Water = data[5], Terrain = data[6], _7 = data[7] },
				ElementType.Track => new TrackElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
				ElementType.Tree => new TreeElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
				ElementType.Wall => new WallElement() { Type = Type, Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
				_ => throw new NotImplementedException(),
			};
		}
	}

	public class BuildingElement : TileElement
	{
		public uint8_t _4 { get; set; }
		public uint8_t _5 { get; set; }
		public uint16_t _6 { get; set; }
	}

	public class IndustryElement : TileElement
	{
		public uint8_t IndustryId { get; set; }
		public uint8_t _5 { get; set; }
		public uint16_t _6 { get; set; }
	}

	public class RoadElement : TileElement
	{
		public uint8_t _4 { get; set; }
		public uint8_t _5 { get; set; }
		public uint8_t _6 { get; set; }
		public uint8_t _7 { get; set; }
	}

	public class SignalElement : TileElement
	{
		public class Side
		{
			public uint8_t _4 { get; set; }
			public uint8_t _5 { get; set; }
		}

		public Side LeftSide { get; set; }
		public Side RightSide { get; set; }
	}

	public class StationElement : TileElement
	{
		public uint8_t _4 { get; set; }
		public uint8_t _5 { get; set; }
		public uint16_t StationId { get; set; }
	}

	public class SurfaceElement : TileElement
	{
		public uint8_t Slope { get; set; }
		public uint8_t Water { get; set; }
		public uint8_t Terrain { get; set; }
		public uint8_t _7 { get; set; }

		public bool IsWater() => (Water & 0x1F) != 0;
		public uint8_t TerrainType() => (uint8_t)(Terrain & 0x1F);
	}

	public class TrackElement : TileElement
	{
		public uint8_t _4 { get; set; }
		public uint8_t _5 { get; set; }
		public uint8_t _6 { get; set; }
		public uint8_t _7 { get; set; }
	}

	public class TreeElement : TileElement
	{
		public uint8_t _4 { get; set; }
		public uint8_t _5 { get; set; }
		public uint8_t _6 { get; set; }
		public uint8_t _7 { get; set; }
	}

	public class WallElement : TileElement
	{
		public uint8_t _4 { get; set; }
		public uint8_t _5 { get; set; }
		public uint8_t _6 { get; set; }
		public uint8_t _7 { get; set; }
	}
}

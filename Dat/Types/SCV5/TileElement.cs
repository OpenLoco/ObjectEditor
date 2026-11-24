// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

using Dat.FileParsing;

namespace Dat.Types.SCV5;

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

	public uint8_t TypeByte { get; set; }
	public ElementType Type
	{
		get => (ElementType)((TypeByte & 0x3C) >> 2);
		set => TypeByte = (uint8_t)((TypeByte & ~0x3C) | (((uint8_t)value << 2) & 0x3C));
	}
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

		var type = (ElementType)((data[0] & 0x3C) >> 2); // https://github.com/OpenLoco/OpenLoco/blob/master/src/OpenLoco/src/Map/Tile.cpp#L23

		return type switch
		{
			ElementType.Building => new BuildingElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = BitConverter.ToUInt16(data[6..8]) },
			ElementType.Industry => new IndustryElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], IndustryId = data[4], _5 = data[5], _6 = BitConverter.ToUInt16(data[6..8]) },
			ElementType.Road => new RoadElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
			ElementType.Signal => new SignalElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], LeftSide = new SignalElement.Side() { _4 = data[4], _5 = data[5] }, RightSide = new SignalElement.Side() { _4 = data[6], _5 = data[7] } },
			ElementType.Station => new StationElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], StationId = BitConverter.ToUInt16(data[6..8]) },
			ElementType.Surface => new SurfaceElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], Slope = data[4], Water = data[5], Terrain = data[6], _7 = data[7] },
			ElementType.Track => new TrackElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
			ElementType.Tree => new TreeElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
			ElementType.Wall => new WallElement() { TypeByte = data[0], Flags = data[1], BaseZ = data[2], ClearZ = data[3], _4 = data[4], _5 = data[5], _6 = data[6], _7 = data[7] },
			_ => throw new NotImplementedException(),
		};
	}

	public byte[] Write()
	{
		var data = new byte[StructLength];
		data[0] = TypeByte;
		data[1] = Flags;
		data[2] = BaseZ;
		data[3] = ClearZ;

		switch (this)
		{
			case BuildingElement x:
				data[4] = x._4;
				data[5] = x._5;
				BitConverter.GetBytes(x._6).CopyTo(data, 6);
				break;
			case IndustryElement x:
				data[4] = x.IndustryId;
				data[5] = x._5;
				BitConverter.GetBytes(x._6).CopyTo(data, 6);
				break;
			case RoadElement x:
				data[4] = x._4;
				data[5] = x._5;
				data[6] = x._6;
				data[7] = x._7;
				break;
			case SignalElement x:
				data[4] = x.LeftSide._4;
				data[5] = x.LeftSide._5;
				data[6] = x.RightSide._4;
				data[7] = x.RightSide._5;
				break;
			case StationElement x:
				data[4] = x._4;
				data[5] = x._5;
				BitConverter.GetBytes(x.StationId).CopyTo(data, 6);
				break;
			case SurfaceElement x:
				data[4] = x.Slope;
				data[5] = x.Water;
				data[6] = x.Terrain;
				data[7] = x._7;
				break;
			case TrackElement x:
				data[4] = x._4;
				data[5] = x._5;
				data[6] = x._6;
				data[7] = x._7;
				break;
			case TreeElement x:
				data[4] = x._4;
				data[5] = x._5;
				data[6] = x._6;
				data[7] = x._7;
				break;
			case WallElement x:
				data[4] = x._4;
				data[5] = x._5;
				data[6] = x._6;
				data[7] = x._7;
				break;
			default:
				throw new NotImplementedException($"Unsupported tile element type {GetType().Name}");
		}

		return data;
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

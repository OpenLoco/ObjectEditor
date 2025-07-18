using Dat.Data;
using Dat.FileParsing;
using System.ComponentModel;

namespace Dat.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(StructLength)]
public class S5Header
{
	// this is necessary because Flags must be get-set to enable setters for SourceGame and ObjectType
	public S5Header(uint32_t flags, string name, uint32_t checksum)
	{
		Flags = flags;
		Name = name.Trim();
		Checksum = checksum;
	}

	public S5Header(string name, uint32_t checksum)
		: this(0, name, checksum)
	{ }

	public S5Header()
		: this(0, string.Empty, uint.MaxValue)
	{ }

	public const int StructLength = 0x10;

	public string Name { get; set; }
	public uint32_t Checksum { get; set; }
	public uint32_t Flags { get; set; }

	public SourceGame SourceGame
	{
		get => (SourceGame)((Flags >> 6) & 0x3u);
		set => Flags |= (Flags & ~(0x3u << 6)) | (((uint)value & 0x3u) << 6);
	}

	public ObjectType ObjectType
	{
		get => (ObjectType)(Flags & 0x3Fu);
		set => Flags |= (Flags & ~0x3Fu) | ((uint)value & 0x3Fu);
	}

	public bool IsValid()
		=> IsValid((int)SourceGame, (int)ObjectType);

	public static bool IsValid(int sourceGame, int objectType)
		=> sourceGame is >= 0 and <= 3 && objectType is >= 0 and < Limits.kMaxObjectTypes;

	public static S5Header Read(ReadOnlySpan<byte> data)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, StructLength);

		var flags = BitConverter.ToUInt32(data[0..4]);
		var name = System.Text.Encoding.ASCII.GetString(data[4..12]);
		var checksum = BitConverter.ToUInt32(data[12..16]);
		return new S5Header(flags, name, checksum);
	}

	public ReadOnlySpan<byte> Write()
	{
		var span = new byte[StructLength];

		var flags = BitConverter.GetBytes(Flags);
		var name = System.Text.Encoding.ASCII.GetBytes(Name.PadRight(8, ' '));
		var checksum = BitConverter.GetBytes(Checksum);

		flags.CopyTo(span, 0);
		name.CopyTo(span, 4);
		checksum.CopyTo(span, 12);
		return span;
	}

	// Vanilla objects do 0x000000FF but all FF is fine too
	public static readonly S5Header NullHeader = new(0x000000FF, "        ", 0);

	public bool IsVanilla()
		=> IsVanilla(Name, Checksum);

	public static bool IsVanilla(string name, uint checksum)
		=> OriginalObjectFiles.GetFileSource(name, checksum) != ObjectSource.Custom;
}

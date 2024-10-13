using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;

// this should really be in OpenLoco/OpenGraphics

var baseDir = "Q:\\Games\\Locomotion\\Palettes";
var paletteFile = Path.Combine(baseDir, "palette.png");
var img = Image.Load<Rgba32>(paletteFile);

MakeJascPal(baseDir, img);
MakeMicrosoftPal(baseDir, img);
MakePhotoshopAco(baseDir, img);
MakePhotoshopAct(baseDir, img);

static void MakeJascPal(string baseDir, Image<Rgba32> img)
{
	// header
	var lines = new List<string>()
	{
		"JASC-PAL", // header
		"0100", // version
		"256", // colours in palette file
	};

	// colours
	for (var y = 0; y < img.Height; y++)
	{
		for (var x = 0; x < img.Width; x++)
		{
			var col = img[x, y];
			lines.Add($"{col.R} {col.G} {col.B}");
		}
	}

	File.WriteAllLines(Path.Combine(baseDir, "locopal.jasc.pal"), lines);
}

static void MakeMicrosoftPal(string baseDir, Image<Rgba32> img)
{
	// from https://www.aelius.com/njh/wavemetatools/doc/riffmci.pdf

	var count = (uint)img.Width * (uint)img.Height;

	using var fs = new FileStream(Path.Combine(baseDir, "locopal.ms.pal"), FileMode.Create, FileAccess.Write, FileShare.None);
	using var bw = new BinaryWriter(fs);

	// RIFF header
	bw.Write(Encoding.ASCII.GetBytes("RIFF"));
	bw.Write(16 + (count * 4)); // exclude RIFF and filesize from the file size calc
	bw.Write(Encoding.ASCII.GetBytes("PAL "));

	// data chunk
	bw.Write(Encoding.ASCII.GetBytes("data"));
	bw.Write(4 + (count * 4)); // chunk size
	bw.Write((ushort)0x0300); // palVersion
	bw.Write((ushort)count); // palNumEntries

	// colours
	for (var y = 0; y < img.Height; y++)
	{
		for (var x = 0; x < img.Width; x++)
		{
			var col = img[x, y];
			bw.Write(col.R); // peRed
			bw.Write(col.G); // peGreen
			bw.Write(col.B); // peBlue
			bw.Write((byte)0x0);   // peFlags (usually 0)
		}
	}
}

static void MakePhotoshopAco(string baseDir, Image<Rgba32> img)
{
	var count = (uint)img.Width * (uint)img.Height;
	using var fs = new FileStream(Path.Combine(baseDir, "locopal.aco"), FileMode.Create, FileAccess.Write, FileShare.None);

	// Write the ACO file header
	WriteInt16(fs, 1); // Version 1
	WriteInt16(fs, (short)count);

	// Write each colour
	for (var y = 0; y < img.Height; y++)
	{
		for (var x = 0; x < img.Width; x++)
		{
			var col = img[x, y];
			WriteInt16(fs, 0); // Colour space: RGB
			WriteInt16(fs, (short)(col.R * 256)); // Red channel
			WriteInt16(fs, (short)(col.G * 256)); // Green channel
			WriteInt16(fs, (short)(col.B * 256)); // Blue channel
			WriteInt16(fs, 0); // Padding
		}
	}

	static void WriteInt16(Stream stream, short value)
	{
		stream.WriteByte((byte)(value >> 8));
		stream.WriteByte((byte)value);
	}
}

static void MakePhotoshopAct(string baseDir, Image<Rgba32> img)
{
	var count = (uint)img.Width * (uint)img.Height;

	// ACT files expect 256 colors
	if (count != 256)
	{
		throw new ArgumentException("ACT files require exactly 256 colours.");
	}

	using var fs = new FileStream(Path.Combine(baseDir, "locopal.act"), FileMode.Create, FileAccess.Write, FileShare.None);

	// Write each colour
	for (var y = 0; y < img.Height; y++)
	{
		for (var x = 0; x < img.Width; x++)
		{
			var col = img[x, y];
			fs.WriteByte(col.R);
			fs.WriteByte(col.G);
			fs.WriteByte(col.B);
		}
	}
}

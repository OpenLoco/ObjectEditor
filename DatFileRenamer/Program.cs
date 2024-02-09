using System.Reflection;

Console.WriteLine("Dat File Renamer v0.1");

// get all files in current exe folder
var currFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
currFolder = "Q:\\Games\\Locomotion\\ttd russia";
var datFiles = Directory.GetFiles(currFolder)
	.Where(f => Path.GetExtension(f).Equals(".dat", StringComparison.OrdinalIgnoreCase));

var count = datFiles.Count();
Console.WriteLine($"Checking {count} files in {currFolder}");

var mismatching = new List<(string originalFile, string datName)>();

foreach (var datFile in datFiles)
{
	// read each files S5Header
	using var fileStream = new FileStream(datFile, FileMode.Open, FileAccess.Read, FileShare.None, 32, FileOptions.SequentialScan | FileOptions.Asynchronous);
	using var reader = new BinaryReader(fileStream);
	var data = reader.ReadBytes(0x10).AsSpan();

	var flags = BitConverter.ToUInt32(data[0..4]);
	var datName = System.Text.Encoding.ASCII.GetString(data[4..12]).Trim();
	var checksum = BitConverter.ToUInt32(data[12..16]);

	var filename = Path.GetFileNameWithoutExtension(datFile).Trim();

	if (filename != datName)
	{
		mismatching.Add((datFile, datName));
		Console.WriteLine($"Mismatch: Filename=\"{filename}\" DatName=\"{datName}\" Checksum={checksum}");
	}
}

Console.WriteLine($"Found {mismatching.Count} file names mismatching");
Console.WriteLine("Would you like to rename these? (Y)es (N)o (A)ll");
var input = Console.ReadLine();

var allAtOnce = string.Equals(input, "a", StringComparison.OrdinalIgnoreCase);
if (string.Equals(input, "y", StringComparison.OrdinalIgnoreCase) || allAtOnce)
{
	foreach (var names in mismatching)
	{
		Console.WriteLine($"{names}");
		var dir = Path.GetDirectoryName(names.originalFile);
		var newFilename = Path.Join(dir, names.datName + Path.GetExtension(names.originalFile));
		Console.WriteLine($"New path is: {newFilename}");

		try
		{
			if (allAtOnce)
			{
				File.Move(names.originalFile, newFilename);
			}
			else
			{
				Console.WriteLine("Rename this file? (Y/N) (Ctrl+C to exit)");
				var inputPerFile = Console.ReadLine();
				if (string.Equals(inputPerFile, "y", StringComparison.OrdinalIgnoreCase))
				{
					File.Move(names.originalFile, newFilename);
				}
			}
		}
		catch (IOException ex)
		{
			Console.WriteLine(ex);
		}
	}
}
else
{
	Console.WriteLine("Rename cancelled");
}

Console.WriteLine("Press any key to exit");
Console.ReadLine();

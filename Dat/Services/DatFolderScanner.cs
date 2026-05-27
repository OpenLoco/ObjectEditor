using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO.Hashing;

namespace Dat.Services;

// Plain result for a single scanned .dat file. Mirrors the metadata the old
// ObjectIndexEntry stored, but lives in the Dat project so it can be produced
// without any database dependencies. The LocalObjectIndexService (in Definitions)
// is responsible for translating these into TblObject + TblDatObject rows.
public record DatFileScanResult(
	string FullPath,
	string RelativePath,
	string DatName,
	uint DatChecksum,
	ulong xxHash3,
	ObjectType ObjectType,
	ObjectSource ObjectSource,
	DateOnly CreatedDate,
	DateOnly ModifiedDate,
	VehicleType? VehicleType);

public record DatFolderScanResults(
	IReadOnlyList<DatFileScanResult> Succeeded,
	IReadOnlyList<string> Failed);

public static class DatFolderScanner
{
	// Parses the in-memory bytes of a single .dat file and returns its metadata, or
	// null if the headers are invalid / parsing fails. Does not touch the disk other
	// than to read file timestamps from absoluteFilename.
	public static DatFileScanResult? TryScanBytes(string absoluteFilename, string relativeFilename, byte[] data, ILogger logger)
	{
		if (!SawyerStreamReader.TryGetHeadersFromBytes(data, out var hdrs, logger))
		{
			logger.LogError("{RelativeFilename} must have valid S5 and Object headers to be scanned", relativeFilename);
			return null;
		}

		var xxHash3 = XxHash3.HashToUInt64(data);
		var source = OriginalObjectFiles.GetFileSource(hdrs.S5.Name, hdrs.S5.Checksum, hdrs.S5.ObjectSource);
		var createdTime = DateOnly.FromDateTime(File.GetCreationTimeUtc(absoluteFilename));
		var modifiedTime = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absoluteFilename));
		var objType = hdrs.S5.ObjectType.Convert();

		VehicleType? vType = null;
		if (objType == ObjectType.Vehicle)
		{
			// only need 4 bytes since vehicle type is in the 4th byte of a vehicle object
			var remainingData = data[(S5Header.StructLength + ObjectHeader.StructLength)..];
			var decoded = SawyerStreamReader.Decode(hdrs.Obj.Encoding, remainingData, 4);
			vType = (VehicleType)decoded[3];
		}

		return new DatFileScanResult(absoluteFilename, relativeFilename, hdrs.S5.Name, hdrs.S5.Checksum, xxHash3, objType, source, createdTime, modifiedTime, vType);
	}

	// Scans every .dat file under the given directory in parallel and returns
	// (succeeded, failed) lists. Files that cannot be read or whose headers are
	// invalid are added to the failed list; everything else produces a scan result.
	public static Task<DatFolderScanResults> ScanDirectoryAsync(string directory, ILogger logger, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
		=> Task.Run(() => ScanDirectory(directory, logger, progress, cancellationToken), cancellationToken);

	public static DatFolderScanResults ScanDirectory(string directory, ILogger logger, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
	{
		var files = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();
		return ScanFiles(directory, files, logger, progress, cancellationToken);
	}

	public static DatFolderScanResults ScanFiles(string directory, IReadOnlyList<string> relativeFiles, ILogger logger, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
	{
		ConcurrentQueue<DatFileScanResult> succeeded = [];
		ConcurrentQueue<string> failed = [];
		var totalFiles = relativeFiles.Count;

		_ = Parallel.ForEach(
			relativeFiles,
			new ParallelOptions { CancellationToken = cancellationToken },
			file => ScanOne(directory, file, succeeded, failed, totalFiles, progress, logger));

		return new DatFolderScanResults([.. succeeded], [.. failed]);
	}

	static void ScanOne(string directory, string relativeFilename, ConcurrentQueue<DatFileScanResult> succeeded, ConcurrentQueue<string> failed, int totalFiles, IProgress<float>? progress, ILogger logger)
	{
		var fullFilename = Path.Combine(directory, relativeFilename);

		if (!File.Exists(fullFilename))
		{
			failed.Enqueue(relativeFilename);
			progress?.Report((succeeded.Count + failed.Count) / (float)totalFiles);
			return;
		}

		DatFileScanResult? entry = null;
		try
		{
			var bytes = File.ReadAllBytes(fullFilename);
			entry = TryScanBytes(fullFilename, relativeFilename, bytes, logger);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to parse file \"{Filename}\"", relativeFilename);
		}

		if (entry == null)
		{
			failed.Enqueue(relativeFilename);
		}
		else
		{
			succeeded.Enqueue(entry);
		}

		progress?.Report((succeeded.Count + failed.Count) / (float)totalFiles);
	}
}

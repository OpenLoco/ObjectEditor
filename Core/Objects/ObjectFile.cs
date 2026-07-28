using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Core.Objects;

public static class ObjectFile
{
	static readonly JsonSerializerOptions jsonOptions = new()
	{
		WriteIndented = true,
	};

	public static LocoObjectFile? Load(string fileName, ILogger logger, PaletteMap? paletteMap = null, bool loadExtra = true)
	{
		ArgumentNullException.ThrowIfNull(logger);

		if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
		{
			logger.LogError("File does not exist: \"{FileName}\"", fileName);
			return null;
		}

		var (datInfo, locoObject) = SawyerStreamReader.LoadFullObject(fileName, logger, loadExtra);

		if (locoObject == null)
		{
			logger.LogError("Unable to load a LocoObject from \"{FileName}\"", fileName);
			return null;
		}

		if (paletteMap != null && locoObject.ImageTable != null)
		{
			locoObject.ImageTable.PaletteMap = paletteMap;
		}

		return new LocoObjectFile(fileName, datInfo, locoObject);
	}

	public static bool SaveDat(LocoObjectFile file, string fileName, ILogger logger, SawyerEncoding? encoding = null, string? objectName = null, ObjectSource? objectSource = null, bool allowSavingAsVanillaObject = false)
	{
		ArgumentNullException.ThrowIfNull(file);
		ArgumentNullException.ThrowIfNull(logger);

		if (!TryPrepareDirectory(fileName, logger))
		{
			return false;
		}

		var header = file.DatInfo.S5Header;

		SawyerStreamWriter.Save(
			fileName,
			objectName ?? header.Name,
			objectSource ?? header.ObjectSource.Convert(header.Name, header.Checksum),
			encoding ?? file.DatInfo.ObjectHeader.Encoding,
			file.LocoObject,
			logger,
			allowSavingAsVanillaObject);

		return true;
	}

	public static bool SaveJson(LocoObjectFile file, string fileName, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(file);
		ArgumentNullException.ThrowIfNull(logger);

		if (!TryPrepareDirectory(fileName, logger))
		{
			return false;
		}

		using var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
		JsonSerializer.Serialize(stream, file.LocoObject, jsonOptions);

		logger.LogInformation("{ObjName} successfully saved to {Filename}", file.DatInfo.S5Header.Name, fileName);
		return true;
	}

	public static IReadOnlyList<string> EnumerateDatFiles(string path, bool recursive = true)
	{
		if (File.Exists(path))
		{
			return [path];
		}

		if (!Directory.Exists(path))
		{
			return [];
		}

		return [.. Directory
			.EnumerateFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
			.Where(x => Path.GetExtension(x).Equals(".dat", StringComparison.OrdinalIgnoreCase))
			.Order()];
	}

	static bool TryPrepareDirectory(string fileName, ILogger logger)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			logger.LogError("Cannot save - filename was empty");
			return false;
		}

		var saveDir = Path.GetDirectoryName(fileName);

		if (string.IsNullOrEmpty(saveDir))
		{
			logger.LogError("Cannot save - directory is null or empty");
			return false;
		}

		if (!Directory.Exists(saveDir))
		{
			logger.LogError("Cannot save - directory does not exist: \"{SaveDir}\"", saveDir);
			return false;
		}

		return true;
	}
}

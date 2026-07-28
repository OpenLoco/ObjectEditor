using Core.Objects;
using Dat.Data;
using Definitions.ObjectModels;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Core.Validation;

public static class ObjectValidation
{
	public static List<string> Validate(ILocoStruct? obj)
		=> [.. (obj?.Validate(new ValidationContext(obj)) ?? []).Select(x => x.ToString() ?? string.Empty)];

	public static List<string> Validate(LocoObjectFile file)
	{
		ArgumentNullException.ThrowIfNull(file);
		return Validate(file.LocoObject.Object);
	}

	public static List<string> ValidateForOG(LocoObjectFile file, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(file);
		ArgumentNullException.ThrowIfNull(logger);

		var validationErrors = new List<string>();

		try
		{
			var fileName = file.FileName;
			if (string.IsNullOrEmpty(fileName))
			{
				validationErrors.Add("Filename is null or empty");
				return validationErrors;
			}

			var currentDir = Path.GetDirectoryName(fileName);
			if (string.IsNullOrEmpty(currentDir))
			{
				validationErrors.Add("Current directory is null or empty");
				return validationErrors;
			}

			// reject if .gitkeep file still exists
			var directoryFiles = Directory.GetFiles(currentDir).Select(x => Path.GetFileName(x)).ToList();
			if (directoryFiles.Contains(".gitkeep"))
			{
				validationErrors.Add("File \".gitkeep\" exists in the current directory");
			}

			// find common textures directory
			var textureDirectory = FindDirectoryInParentDirectory(currentDir, "textures")?.FullName;
			if (string.IsNullOrEmpty(textureDirectory))
			{
				validationErrors.Add("Texture directory name is null or empty");
			}
			else
			{
				// reject if any files are here that existing /textures folder
				var textureFiles = Directory.GetFiles(textureDirectory).Select(x => Path.GetFileName(x));
				foreach (var textureFile in textureFiles)
				{
					if (directoryFiles.Contains(textureFile))
					{
						validationErrors.Add($"File \"{Path.GetFileName(textureFile)}\" exists in both the current directory and the textures directory");
					}
				}
			}

			var header = file.DatInfo.S5Header;
			var currentDirName = Path.GetFileName(currentDir);
			if (OriginalObjectFiles.Names.TryGetValue(currentDirName, out var fileInfo))
			{
				// DAT name is the expected dat name
				if (header.Name != fileInfo.OpenGraphicsName)
				{
					validationErrors.Add($"✖ Internal DAT header name is not correct. Actual=\"{header.Name}\" Expected=\"{fileInfo.OpenGraphicsName}\" ");
				}
			}
			else
			{
				validationErrors.Add($"✖ Unable to find file info for the vanilla file. Name=\"{currentDirName}\".");
			}

			var expectedFilename = $"OG_{currentDirName}.dat";
			var actualFilename = Path.GetFileName(fileName);
			if (expectedFilename != actualFilename)
			{
				validationErrors.Add($"✖ Filename not correct. Actual=\"{actualFilename}\" Expected=\"{expectedFilename}\" ");
			}

			// DAT name is NOT prefixed by OG_
			if (header.Name.Contains('_'))
			{
				validationErrors.Add("✖ Internal header name should not contain an underscore");
			}

			// DAT name is prefixed by OG
			if (!header.Name.StartsWith("OG"))
			{
				validationErrors.Add("✖ Internal header name is not prefixed with OG");
			}

			// OpenGraphics object source set
			if (header.ObjectSource != DatObjectSource.OpenLoco)
			{
				validationErrors.Add("✖ Object source is not set to OpenLoco");
			}

			// if Vehicle - use RunLengthSingle
			if (header.ObjectType == DatObjectType.Vehicle && file.DatInfo.ObjectHeader.Encoding != SawyerEncoding.RunLengthSingle)
			{
				validationErrors.Add("✖ Object is a Vehicle but doesn't have encoding set to RunLengthSingle");
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error validating for OpenGraphics");
			validationErrors.Add($"Error validating for OpenGraphics: {ex.Message}");
		}

		return validationErrors;
	}

	public static DirectoryInfo? FindDirectoryInParentDirectory(string startPath, string targetName)
	{
		var current = new DirectoryInfo(startPath);

		while (current != null)
		{
			foreach (var dir in current.EnumerateDirectories(targetName, SearchOption.TopDirectoryOnly))
			{
				if (string.Equals(dir.Name, targetName, StringComparison.OrdinalIgnoreCase))
				{
					return dir;
				}
			}

			// Move up to the parent directory
			current = current.Parent;
		}

		return null; // Reached root without finding the target directory
	}
}

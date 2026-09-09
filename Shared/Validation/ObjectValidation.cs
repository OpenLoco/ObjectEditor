using Dat.Data;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using Shared.Files;
using System.ComponentModel.DataAnnotations;

namespace Shared.Validation;

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
					validationErrors.Add($"Internal DAT header name is not correct. Actual=\"{header.Name}\" Expected=\"{fileInfo.OpenGraphicsName}\" ");
				}
			}
			else
			{
				validationErrors.Add($"Unable to find file info for the vanilla file. Name=\"{currentDirName}\".");
			}

			string[] expectedPrefixes = ["OG", "EX"];

			foreach (var expectedPrefix in expectedPrefixes)
			{
				var expectedFilename = $"{expectedPrefix}_{currentDirName}.dat";
				var actualFilename = Path.GetFileName(fileName);
				if (expectedFilename != actualFilename)
				{
					validationErrors.Add($"Filename not correct. Actual=\"{actualFilename}\" Expected=\"{expectedFilename}\" ");
				}
			}

			// DAT name is NOT prefixed by OG_
			if (header.Name.Contains('_'))
			{
				validationErrors.Add("Internal header name should not contain an underscore");
			}

			// DAT name is prefixed by OG
			if (!header.Name.StartsWith("OG") && !header.Name.StartsWith("EX"))
			{
				validationErrors.Add("Internal header name is not prefixed with OG or EX");
			}

			// OpenGraphics object source set
			if (header.ObjectSource != DatObjectSource.OpenLoco)
			{
				validationErrors.Add("Object source is not set to OpenLoco");
			}

			// if Vehicle - use RunLengthSingle
			if (header.ObjectType == DatObjectType.Vehicle && file.DatInfo.ObjectHeader.Encoding != SawyerEncoding.RunLengthSingle)
			{
				validationErrors.Add("Object is a Vehicle but doesn't have encoding set to RunLengthSingle");
			}
		}
		catch (IOException ex)
		{
			logger.LogError(ex, "I/O error validating for OpenGraphics");
			validationErrors.Add($"Error validating for OpenGraphics: {ex.Message}");
		}
		catch (UnauthorizedAccessException ex)
		{
			logger.LogError(ex, "Access denied during OpenGraphics validation");
			validationErrors.Add($"Error validating for OpenGraphics: {ex.Message}");
		}

		return validationErrors.Select(x => $"✖ {x}").ToList();
	}

	/// <summary>
	/// Returns the object headers that <paramref name="obj"/> depends on being present in the same
	/// scenario. Objects without cross-object dependencies return an empty collection.
	/// </summary>
	public static IEnumerable<ObjectModelHeader> GetObjectDependencies(ILocoStruct? obj)
		=> obj switch
		{
			IndustryObject industry => [.. industry.ProducedCargo, .. industry.RequiredCargo, .. industry.WallTypes, .. Maybe(industry.BuildingWall), .. Maybe(industry.BuildingWallEntrance)],
			BuildingObject building => [.. building.ProducedCargoType, .. building.ConsumedCargoType],
			BridgeObject bridge => [.. bridge.CompatibleTrackObjects, .. bridge.CompatibleRoadObjects],
			LandObject land => [land.CliffEdgeHeader, .. Maybe(land.ReplacementLandHeader)],
			RegionObject region => [.. region.CargoInfluenceObjects, .. region.DependentObjects],
			RoadObject road => [road.Tunnel, .. road.Bridges, .. road.Stations, .. road.RoadMods, .. road.TracksAndRoads],
			RoadStationObject roadStation => [.. roadStation.CompatibleRoadObjects, .. Maybe(roadStation.CargoType)],
			SteamObject steam => [.. steam.SoundEffects],
			TrackObject track => [track.Tunnel, .. track.Bridges, .. track.Stations, .. track.Signals, .. track.TrackMods, .. track.TracksAndRoads],
			TrackSignalObject trackSignal => [.. trackSignal.CompatibleTrackObjects],
			TrackStationObject trackStation => [.. trackStation.CompatibleTrackObjects],
			VehicleObject vehicle => GetVehicleDependencies(vehicle),

			_ => [],
		};

	static IEnumerable<ObjectModelHeader> GetVehicleDependencies(VehicleObject vehicle)
	{
		foreach (var emitter in vehicle.ParticleEmitters)
		{
			yield return emitter.AnimationObject;
		}

		foreach (var sound in new[] { vehicle.FrictionSound?.SoundObject, vehicle.SimpleMotorSound?.SoundObject, vehicle.GearboxMotorSound?.SoundObject })
		{
			if (sound != null)
			{
				yield return sound;
			}
		}

		if (vehicle.RoadOrTrackType != null)
		{
			yield return vehicle.RoadOrTrackType;
		}

		if (vehicle.RackRail != null)
		{
			yield return vehicle.RackRail;
		}

		if (vehicle.DrivingSound != null)
		{
			yield return vehicle.DrivingSound;
		}

		foreach (var obj in vehicle.CompatibleVehicles)
		{
			yield return obj;
		}

		foreach (var obj in vehicle.RequiredTrackExtras)
		{
			yield return obj;
		}

		foreach (var obj in vehicle.StartSounds)
		{
			yield return obj;
		}

		foreach (var obj in vehicle.CrossingSounds)
		{
			yield return obj;
		}
	}


	static IEnumerable<ObjectModelHeader> Maybe(ObjectModelHeader? header)
		=> header == null ? [] : [header];

	/// <summary>
	/// Validates that every object dependency required by the objects included in a scenario or
	/// save file is itself present in the file's required-object list. For example, if a
	/// file includes an industry that produces cargo B and consumes cargo C, then the
	/// file must also include cargo B and C to be valid.
	/// </summary>
	/// <param name="fileObjects">
	/// The objects included in this file (e.g. an <c>S5File</c>'s required objects, converted
	/// to <see cref="ObjectModelHeader"/>s).
	/// </param>
	/// <param name="objectDependencyResolver">
	/// Given an object that is included in the scenario or save file, returns the object headers
	/// that it depends on being present (e.g. an industry's produced and consumed cargo). Return
	/// an empty collection for objects with no dependencies. This is decoupled from any specific
	/// source so that the validation is unit-testable and can be called from the CLI and GUI
	/// alike.
	/// </param>
	public static List<string> ValidateSCV5(
		IEnumerable<ObjectModelHeader> fileObjects,
		Func<ObjectModelHeader, IEnumerable<ObjectModelHeader>> objectDependencyResolver)
	{
		ArgumentNullException.ThrowIfNull(fileObjects);
		ArgumentNullException.ThrowIfNull(objectDependencyResolver);

		var validationErrors = new List<string>();
		ValidateDependentObjects(fileObjects, objectDependencyResolver, validationErrors);

		return validationErrors;

		static string GetPrintableName(ObjectModelHeader omh)
			=> $"[{omh.Name} | {omh.ObjectType} | 0x{omh.DatChecksum:X8}]";

		static void ValidateDependentObjects(IEnumerable<ObjectModelHeader> fileObjects, Func<ObjectModelHeader, IEnumerable<ObjectModelHeader>> objectDependencyResolver, List<string> validationErrors)
		{
			// Unique objects that are actually included in the file (dedupe and skip empty placeholders).
			var includedObjects = fileObjects
				.Where(x => x is not null && !string.IsNullOrWhiteSpace(x.Name))
				.GroupBy(x => x.Name)
				.Select(x => x.First())
				.ToList();

			var includedKeys = includedObjects
				.Select(x => x.Name)
				.ToHashSet();

			foreach (var obj in includedObjects)
			{
				IEnumerable<ObjectModelHeader> dependencies;
				try
				{
					dependencies = objectDependencyResolver(obj) ?? [];
				}
				catch (Exception)
				{
					// A single unresolvable object shouldn't block validation of the rest of the scenario.
					dependencies = [];
				}

				foreach (var dependency in dependencies.Where(x => x is not null && !string.IsNullOrWhiteSpace(x.Name)))
				{
					if (includedKeys.Contains(dependency.Name))
					{
						continue;
					}

					validationErrors.Add(
						$"✘ Object {GetPrintableName(obj)} requires dependency {GetPrintableName(dependency)} to be included, but it is not present in this file's required objects.");
				}
			}
		}
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

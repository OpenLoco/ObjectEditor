using Common.Json;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Core.Graphics;

public static class ImageTableIo
{
	public const string SpritesFileName = "sprites.json";

	public static async Task<int> ExportAsync(ImageTable imageTable, string directory, bool prependGroupAndImageNameInFilename, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(imageTable);
		ArgumentNullException.ThrowIfNull(logger);

		if (string.IsNullOrEmpty(directory))
		{
			logger.LogError("Directory is invalid: \"{Directory}\"", directory);
			return 0;
		}

		_ = Directory.CreateDirectory(directory);

		logger.LogInformation("Exporting images to {Directory}", directory);

		var offsets = new List<GraphicsElementJson>();
		var invalidChars = Path.GetInvalidFileNameChars();

		foreach (var item in imageTable.Groups
			.SelectMany(group => group.GraphicsElements, (group, element) => new { group.Name, Element = element })
			.OrderBy(x => x.Element.ImageTableIndex))
		{
			var element = item.Element;

			var fileName = $"{element.ImageTableIndex}.png";
			if (prependGroupAndImageNameInFilename)
			{
				var imageName = Sanitize(element.Name, invalidChars);
				var groupName = Sanitize(item.Name, invalidChars);

				if (!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(imageName))
				{
					fileName = $"{groupName}_{imageName}.png";
				}
			}

			if (element.Image == null)
			{
				logger.LogWarning("Image[{Index}] has no decoded image and will be skipped", element.ImageTableIndex);
				continue;
			}

			await element.Image.SaveAsPngAsync(Path.Combine(directory, fileName));
			offsets.Add(new GraphicsElementJson(fileName, element));
		}

		var offsetsFile = Path.Combine(directory, SpritesFileName);
		logger.LogInformation("Saving sprite offsets to {OffsetsFile}", offsetsFile);
		await JsonFile.SerializeToFileAsync(offsets, offsetsFile);

		return offsets.Count;

		static string Sanitize(string value, char[] invalidChars)
			=> new string([.. value.ToLower().Replace(' ', '-').Where(x => !invalidChars.Contains(x))]).Trim();
	}

	public static async Task<ICollection<GraphicsElementJson>?> LoadSpritesJsonAsync(string filename, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(logger);

		if (!File.Exists(filename))
		{
			return null;
		}

		var offsets = await JsonFile.DeserializeFromFileAsync<ICollection<GraphicsElementJson>>(filename);
		logger.LogDebug("Found sprites.json file with {Count} images", offsets?.Count ?? 0);
		return offsets;
	}

	public static async Task<List<GraphicsElement>?> LoadImagesAsync(string directory, PaletteMap paletteMap, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(logger);

		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			logger.LogError("Directory does not exist: \"{Directory}\"", directory);
			return null;
		}

		var spritesFile = Path.Combine(directory, SpritesFileName);
		var sprites = await LoadSpritesJsonAsync(spritesFile, logger);

		if (sprites == null || sprites.Count == 0)
		{
			logger.LogError("No sprites.json found or file is empty in {Directory}. Import aborted.", directory);
			return null;
		}

		var importedImages = new List<GraphicsElement>();
		foreach (var (sprite, i) in sprites.Select((x, i) => (x, i)))
		{
			var is1Pixel = string.IsNullOrEmpty(sprite.Path);
			var img = is1Pixel
				? ImageTableHelpers.OnePixelTransparent
				: Image.Load<Rgba32>(Path.Combine(directory, sprite.Path));

			var effectiveSprite = is1Pixel
				? sprite with { Flags = GraphicsElementFlags.HasTransparency }
				: sprite;

			var graphicsElement = GraphicsElementOperations.FromImage(effectiveSprite, img, paletteMap, i);
			graphicsElement.Name = string.IsNullOrEmpty(graphicsElement.Name)
				? DefaultImageTableNameProvider.GetImageName(i)
				: graphicsElement.Name;

			importedImages.Add(graphicsElement);
		}

		return importedImages;
	}

	public static async Task<int> ImportAsync(ImageTable imageTable, string directory, PaletteMap paletteMap, ILogger logger, ILocoStruct? objectModel = null, ObjectType? objectType = null)
	{
		ArgumentNullException.ThrowIfNull(imageTable);

		logger.LogInformation("Importing images from {Directory}", directory);

		var importedImages = await LoadImagesAsync(directory, paletteMap, logger);
		if (importedImages == null)
		{
			return 0;
		}

		imageTable.Groups.Clear();
		imageTable.Groups.Add(new ImageTableGroup("<temp>", importedImages));

		Regroup(imageTable, logger, objectModel, objectType);

		return importedImages.Count;
	}

	public static void Regroup(ImageTable imageTable, ILogger logger, ILocoStruct? objectModel, ObjectType? objectType)
	{
		ArgumentNullException.ThrowIfNull(imageTable);

		if (objectModel == null || !objectType.HasValue)
		{
			return;
		}

		var imageList = imageTable.GraphicsElements;

		try
		{
			imageTable.Groups = [.. ImageTableGrouper.CreateGroupsForExistingImages(objectModel, objectType.Value, imageList)];
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to regroup the image table - images will remain in a single flat group");
		}
	}

	public static async Task<int> ApplyOffsetsAsync(ImageTable imageTable, string spritesJsonFileName, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(imageTable);

		var offsets = await LoadSpritesJsonAsync(spritesJsonFileName, logger);
		if (offsets == null)
		{
			logger.LogError("Failed to load offsets from {Filename}", spritesJsonFileName);
			return 0;
		}

		var elements = imageTable.GraphicsElements;
		var applied = 0;

		foreach (var (offset, i) in offsets.Select((o, index) => (o, index)))
		{
			if (elements.Count <= i)
			{
				logger.LogError("Offset for Image[{Index}] is provided in the sprites.json file, but only {Count} images are available in the current image table. This offset will be skipped.", i, elements.Count);
				continue;
			}

			elements[i].XOffset = offset.XOffset;
			elements[i].YOffset = offset.YOffset;
			applied++;
		}

		return applied;
	}
}

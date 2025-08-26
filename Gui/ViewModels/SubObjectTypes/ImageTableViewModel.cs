using Avalonia.Controls.Selection;
using Avalonia.Threading;
using Common;
using Common.Json;
using Common.Logging;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gui.ViewModels;

public class ImageTableViewModel : ReactiveObject, IExtraContentViewModel
{
	public string Name => "Image Table";

	// used in the axaml to bind the combobox to the list of swatches
	public static ColourRemapSwatch[] ColourSwatchesArr { get; } = Enum.GetValues<ColourRemapSwatch>();

	[Reactive]
	public ColourRemapSwatch SelectedPrimarySwatch { get; set; } = ColourRemapSwatch.PrimaryRemap;

	[Reactive]
	public ColourRemapSwatch SelectedSecondarySwatch { get; set; } = ColourRemapSwatch.SecondaryRemap;

	[Reactive]
	public ICommand ReplaceImageCommand { get; set; }

	[Reactive]
	public ICommand ImportImagesCommand { get; set; }

	[Reactive]
	public ICommand ExportImagesCommand { get; set; }

	[Reactive]
	public ICommand CropAllImagesCommand { get; set; }

	[Reactive]
	public ICommand ZeroOffsetAllImagesCommand { get; set; }

	[Reactive]
	public ICommand CenterOffsetAllImagesCommand { get; set; }

	// what is displaying on the ui
	[Reactive]
	public ObservableCollection<ImageViewModel> ImageViewModels { get; set; } = [];

	[Reactive]
	public int SelectedImageIndex { get; set; } = -1;

	[Reactive]
	public SelectionModel<ImageViewModel> SelectionModel { get; set; }

	[Reactive]
	public ImageViewModel? SelectedImage { get; set; }

	[Reactive]
	public int AnimationSpeed { get; set; } = 40;
	readonly DispatcherTimer animationTimer;
	int currentFrameIndex;

	public readonly IImageTableNameProvider NameProvider; // Can remove this when we put name inside of GraphicsElement
	public PaletteMap PaletteMap { get; init; }
	public readonly ILogger Logger;

	public ImageTableViewModel(IList<GraphicsElement> graphicsElements, IImageTableNameProvider imageNameProvider, PaletteMap paletteMap, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(paletteMap);

		var index = 0;
		foreach (var ge in graphicsElements)
		{
			var success = imageNameProvider.TryGetImageName(index, out var imageName);
			ImageViewModels.Add(new ImageViewModel(index, success ? imageName! : "failed to get image name", ge, paletteMap));
			index++;
		}

		NameProvider = imageNameProvider;
		PaletteMap = paletteMap;
		Logger = logger;

		SelectionModel = new SelectionModel<ImageViewModel>
		{
			SingleSelect = false
		};
		SelectionModel.SelectionChanged += SelectionChanged;

		_ = this.WhenAnyValue(o => o.SelectedPrimarySwatch).Skip(1)
			.Subscribe(_ => RecolourImages(SelectedPrimarySwatch, SelectedSecondarySwatch));
		_ = this.WhenAnyValue(o => o.SelectedSecondarySwatch).Skip(1)
			.Subscribe(_ => RecolourImages(SelectedPrimarySwatch, SelectedSecondarySwatch));

		_ = this.WhenAnyValue(o => o.SelectedImageIndex)
			.Subscribe(index =>
			{
				SelectedImage = index < 0 || index >= ImageViewModels.Count
				? null
				: ImageViewModels[SelectedImageIndex];
			});

		_ = this.WhenAnyValue(o => o.AnimationSpeed)
			.Where(_ => animationTimer != null)
			.Subscribe(_ => animationTimer!.Interval = TimeSpan.FromMilliseconds(1000 / AnimationSpeed));

		ImportImagesCommand = ReactiveCommand.CreateFromTask(ImportImages);
		ExportImagesCommand = ReactiveCommand.CreateFromTask(ExportImages);
		ReplaceImageCommand = ReactiveCommand.CreateFromTask(ReplaceImage);
		CropAllImagesCommand = ReactiveCommand.Create(CropAllImages);

		ZeroOffsetAllImagesCommand = ReactiveCommand.Create(() =>
		{
			foreach (var ivm in ImageViewModels)
			{
				ivm.XOffset = 0;
				ivm.YOffset = 0;
			}
		});

		CenterOffsetAllImagesCommand = ReactiveCommand.Create(() =>
		{
			foreach (var ivm in ImageViewModels)
			{
				ivm.XOffset = (short)(-ivm.Width / 2);
				ivm.YOffset = (short)(-ivm.Height / 2);
			}
		});

		// Set up the animation timer
		animationTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(25) // Adjust animation speed as needed
		};
		animationTimer.Tick += AnimationTimer_Tick;
		animationTimer.Start();
	}

	void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
	{
		var sm = (SelectionModel<ImageViewModel>)sender;

		if (sm.SelectedIndexes.Count > 0)
		{
			SelectedImageIndex = sm.SelectedIndex;
		}

		if (sm.SelectedItems.Count == 0)
		{
			return;
		}
	}

	void AnimationTimer_Tick(object? sender, EventArgs e)
	{
		if (SelectionModel == null || SelectionModel.SelectedIndexes.Count == 0)
		{
			return;
		}

		if (currentFrameIndex >= SelectionModel.SelectedIndexes.Count)
		{
			currentFrameIndex = 0;
		}

		// Update the displayed image viewmodel
		SelectedImageIndex = SelectionModel.SelectedIndexes[currentFrameIndex]; // disabling this also makes the memory leaks stop

		// Move to the next frame, looping back to the beginning if necessary
		currentFrameIndex = (currentFrameIndex + 1) % SelectionModel.SelectedIndexes.Count;
	}

	public void ClearSelectionModel()
	{
		SelectionModel.Clear();
		SelectedImageIndex = -1;
	}

	public async Task ImportImages()
	{
		animationTimer.Stop();

		var folders = await PlatformSpecific.OpenFolderPicker();
		using (var dir = folders.FirstOrDefault())
		{
			if (dir == null)
			{
				return;
			}

			var dirPath = dir.Path.LocalPath;
			await ImportImages(dirPath);
		}

		animationTimer.Start();
	}

	public async Task ExportImages()
	{
		var folders = await PlatformSpecific.OpenFolderPicker();
		var dir = folders.FirstOrDefault();
		if (dir == null)
		{
			return;
		}

		var dirPath = dir.Path.LocalPath;
		await ExportImages(dirPath);
	}

	public async Task ReplaceImage()
	{
		if (SelectedImageIndex == -1)
		{
			return;
		}

		var openFile = await PlatformSpecific.OpenFilePicker(PlatformSpecific.PngFileTypes);
		if (openFile == null)
		{
			return;
		}

		var filename = openFile.SingleOrDefault()?.Path.LocalPath;
		if (filename == null)
		{
			return;
		}

		ImageViewModels[SelectedImageIndex].UnderlyingImage = Image.Load<Rgba32>(filename);
	}

	// model stuff
	public void RecolourImages(ColourRemapSwatch primary, ColourRemapSwatch secondary)
	{
		foreach (var ivm in ImageViewModels)
		{
			ivm.RecolourImage(primary, secondary);
		}
	}

	public void CropAllImages()
	{
		foreach (var ivm in ImageViewModels)
		{
			ivm.CropImage();
		}
	}

	public string GetImageName(int index)
		=> NameProvider.TryGetImageName(index, out var value) && !string.IsNullOrEmpty(value)
			? value
			: index.ToString();

	public static string TrimZeroes(string str)
	{
		var result = str.Trim().TrimStart('0');
		return result.Length == 0 ? "0" : result;
	}

	public async Task ImportImages(string directory)
	{
		if (string.IsNullOrEmpty(directory))
		{
			Logger.Error($"Directory is invalid: \"{directory}\"");
			return;
		}

		if (!Directory.Exists(directory))
		{
			Logger.Error($"Directory does not exist: \"{directory}\"");
			return;
		}

		Logger.Info($"Importing images from {directory}");

		ClearSelectionModel();

		try
		{
			Logger.Debug($"{ImageViewModels.Count} images in current object");
			ICollection<GraphicsElementJson> offsets;

			// check for offsets file
			var offsetsFile = Path.Combine(directory, "sprites.json");
			if (File.Exists(offsetsFile))
			{
				offsets = await JsonFile.DeserializeFromFileAsync<ICollection<GraphicsElementJson>>(offsetsFile); // sprites.json is an unnamed array so we need ICollection here, not IEnumerable
				ArgumentNullException.ThrowIfNull(offsets);
				Logger.Debug($"Found sprites.json file with {offsets.Count} images");
			}
			else
			{
				var files = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);
				var sanitised = files.Select(TrimZeroes).ToList();

				offsets = [.. ImageViewModels
					.Select((x, i) => new GraphicsElementJson($"{sanitised[i]}.png", (short)x.XOffset, (short)x.YOffset))
					.Fill(files.Length, GraphicsElementJson.Zero)];

				Logger.Debug($"Didn't find sprites.json file, using existing G1Element32 offsets with {offsets.Count} images");
			}

			// clear existing images
			Logger.Debug("Clearing current images");
			ImageViewModels.Clear();

			// load files
			foreach (var (offset, i) in offsets.Select((x, i) => (x, i)))
			{
				var is1Pixel = string.IsNullOrEmpty(offset.Path);
				var img = is1Pixel ? OnePixelTransparent : Image.Load<Rgba32>(Path.Combine(directory, offset.Path));
				var newOffset = is1Pixel ? offset with { Flags = GraphicsElementFlags.HasTransparency } : offset;
				var graphicsElement = GraphicsElementFromImage(newOffset, img, PaletteMap);

				_ = NameProvider.TryGetImageName(i, out var imageName);
				ImageViewModels.Add(new ImageViewModel(i, imageName ?? "<null>", graphicsElement, PaletteMap));
			}

			Logger.Debug($"Imported {ImageViewModels.Count} images successfully");
		}
		catch (Exception ex)
		{
			Logger.Error(ex);
		}
	}

	static GraphicsElement GraphicsElementFromImage(GraphicsElementJson ele, Image<Rgba32> img, PaletteMap paletteMap)
	{
		var flags = ele.Flags ?? GraphicsElementFlags.None;
		return new GraphicsElement()
		{
			Width = (int16_t)img.Width,
			Height = (int16_t)img.Height,
			XOffset = ele.XOffset,
			YOffset = ele.YOffset,
			Flags = flags,
			ZoomOffset = ele.ZoomOffset ?? 0,
			ImageData = paletteMap.ConvertRgba32ImageToG1Data(img, flags)
		};
	}

	static readonly Image<Rgba32> OnePixelTransparent = new(1, 1, PaletteMap.Transparent.Color);

	public async Task ExportImages(string directory)
	{
		if (string.IsNullOrEmpty(directory))
		{
			Logger.Error($"Directory is invalid: \"{directory}\"");
			return;
		}

		if (!Directory.Exists(directory))
		{
			Logger.Error($"Directory does not exist: \"{directory}\"");
			return;
		}

		Logger.Info($"Exporting images to {directory}");

		var counter = 0;
		var offsets = new List<GraphicsElementJson>();

		foreach (var image in ImageViewModels)
		{
			var imageName = counter.ToString(); // todo: maybe use image name provider below (but number must still exist)
			counter++;

			var fileName = $"{imageName}.png";
			var path = Path.Combine(directory, fileName);
			await image.UnderlyingImage.SaveAsPngAsync(path);

			offsets.Add(new GraphicsElementJson(fileName, image.ToGraphicsElement()));
		}

		var offsetsFile = Path.Combine(directory, "sprites.json");
		Logger.Info($"Saving sprite offsets to {offsetsFile}");
		await JsonFile.SerializeToFileAsync(offsets, offsetsFile);
	}
}

using Avalonia.Controls.Selection;
using Avalonia.Threading;
using Common;
using Common.Json;
using Common.Logging;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Common;
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

namespace Gui.ViewModels.Graphics;

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
	public ICommand ImportImagesCommand { get; set; }

	[Reactive]
	public ICommand ExportImagesCommand { get; set; }
	[Reactive]
	public ICommand ReplaceImageCommand { get; set; }

	[Reactive]
	public ICommand CropImageCommand { get; set; }

	[Reactive]
	public ICommand CropAllImagesCommand { get; set; }

	[Reactive]
	public ICommand ZeroOffsetAllImagesCommand { get; set; }

	[Reactive]
	public ICommand CenterOffsetAllImagesCommand { get; set; }

	[Reactive]
	public SelectionModel<ImageViewModel> SelectionModel { get; set; }

	[Reactive]
	public ImageViewModel? SelectedImage { get; set; }

	[Reactive]
	public int AnimationSpeed { get; set; } = 40;
	readonly DispatcherTimer animationTimer;
	int currentFrameIndex;

	public PaletteMap PaletteMap { get; init; }
	public readonly ILogger Logger;

	[Reactive]
	public ObservableCollection<GroupedImageViewModel> GroupedImageViewModels { get; set; } = [];

	[Reactive]
	public ObservableCollection<ImageViewModel> LayeredImages { get; set; } = [];

	[Reactive]
	public int OffsetSpacing { get; set; }
	int prevOffset;

	public ImageTableViewModel(ImageTable imageTable, PaletteMap paletteMap, ILogger logger, BuildingComponents buildingComponents = null)
	{
		ArgumentNullException.ThrowIfNull(paletteMap);

		foreach (var group in imageTable.Groups)
		{
			var imageViewModels = group.GraphicsElements.Select(ge => new ImageViewModel(ge, paletteMap));
			var givm = new GroupedImageViewModel(group.Name, imageViewModels);
			givm.SelectionModel.SelectionChanged += SelectionChanged;
			GroupedImageViewModels.Add(givm);

		}

		// building components
		if (buildingComponents != null)
		{
			var allGEs = imageTable.GraphicsElements;
			var baseY = 128;

			foreach (var variation in buildingComponents.BuildingVariations)
			{
				var yOffset = 0;
				foreach (var variationItem in variation)
				{
					var group = imageTable.Groups[variationItem];
					var numDirections = 4;
					for (var i = 0; i < numDirections; ++i)
					{
						var x = new ImageViewModel(group.GraphicsElements[i], paletteMap);
						x.XOffset += (i + 1) * 128;
						x.YOffset += baseY + yOffset;
						LayeredImages.Add(x);
					}

					yOffset -= buildingComponents.BuildingHeights[variationItem];
				}

				baseY += 128;
			}
		}

		_ = this.WhenAnyValue(x => x.OffsetSpacing)
			.Where(_ => LayeredImages.Count > 0)
			.Subscribe(spacing =>
			{
				var index = 0;
				foreach (var img in LayeredImages)
				{
					var diff = OffsetSpacing - prevOffset;
					img.YOffset -= diff * index;
					img.RaisePropertyChanged(nameof(img.YOffset));
					img.RaisePropertyChanged(nameof(img));
					index++;
				}

				prevOffset = OffsetSpacing;
				this.RaisePropertyChanged(nameof(LayeredImages));
			});

		PaletteMap = paletteMap;
		Logger = logger;

		_ = this.WhenAnyValue(o => o.SelectedPrimarySwatch).Skip(1)
			.Subscribe(_ => RecolourImages(SelectedPrimarySwatch, SelectedSecondarySwatch));
		_ = this.WhenAnyValue(o => o.SelectedSecondarySwatch).Skip(1)
			.Subscribe(_ => RecolourImages(SelectedPrimarySwatch, SelectedSecondarySwatch));
		_ = this.WhenAnyValue(o => o.AnimationSpeed)
			.Where(_ => animationTimer != null)
			.Subscribe(_ => animationTimer!.Interval = TimeSpan.FromMilliseconds(1000 / AnimationSpeed));

		ImportImagesCommand = ReactiveCommand.CreateFromTask(ImportImages);
		ExportImagesCommand = ReactiveCommand.CreateFromTask(ExportImages);
		ReplaceImageCommand = ReactiveCommand.CreateFromTask(ReplaceImage);
		CropImageCommand = ReactiveCommand.Create(CropImage);
		CropAllImagesCommand = ReactiveCommand.Create(CropAllImages);

		ZeroOffsetAllImagesCommand = ReactiveCommand.Create(() =>
		{
			foreach (var ivm in GroupedImageViewModels.SelectMany(x => x.Images))
			{
				ivm.XOffset = 0;
				ivm.YOffset = 0;
			}
		});

		CenterOffsetAllImagesCommand = ReactiveCommand.Create(() =>
		{
			foreach (var ivm in GroupedImageViewModels.SelectMany(x => x.Images))
			{
				ivm.XOffset = (short)(-ivm.Width / 2);
				ivm.YOffset = (short)(-ivm.Height / 2);
			}
		});

		SelectionModel = new SelectionModel<ImageViewModel>
		{
			SingleSelect = false
		};
		SelectionModel.SelectionChanged += SelectionChanged;

		// Set up the animation timer
		animationTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(25) // Adjust animation speed as needed
		};
		animationTimer.Tick += AnimationTimer_Tick;
		animationTimer.Start();
	}

	void AnimationTimer_Tick(object? sender, EventArgs e)
	{
		if (SelectionModel == null || SelectionModel.SelectedItems.Count == 0)
		{
			return;
		}

		if (currentFrameIndex >= SelectionModel.SelectedItems.Count)
		{
			currentFrameIndex = 0;
		}

		// Update the displayed image viewmodel
		SelectedImage = SelectionModel.SelectedItems[currentFrameIndex]; // disabling this also makes the memory leaks stop

		// Move to the next frame, looping back to the beginning if necessary
		currentFrameIndex = (currentFrameIndex + 1) % SelectionModel.SelectedItems.Count;
	}

	void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
	{
		var sm = (SelectionModel<ImageViewModel>)sender;
		SelectionModel = sm;
		SelectedImage = SelectionModel.SelectedItems.Count > 0 ? sm.SelectedItems[0] : null;
	}

	public void ClearSelectionModel()
	{
		foreach (var givm in GroupedImageViewModels)
		{
			givm.SelectionModel.Clear();
		}

		SelectionModel.Clear();
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

		_ = SelectedImage?.UnderlyingImage = Image.Load<Rgba32>(filename);
	}

	// model stuff
	public void RecolourImages(ColourRemapSwatch primary, ColourRemapSwatch secondary)
	{
		foreach (var ivm in GroupedImageViewModels.SelectMany(x => x.Images))
		{
			ivm.RecolourImage(primary, secondary);
		}
	}

	public void CropImage()
		=> SelectedImage?.CropImage();

	public void CropAllImages()
	{
		foreach (var ivm in GroupedImageViewModels.SelectMany(x => x.Images))
		{
			ivm.CropImage();
		}
	}

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

				offsets = [.. GroupedImageViewModels.SelectMany(x => x.Images)
					.Select((x, i) => new GraphicsElementJson(sanitised[i], (short)x.XOffset, (short)x.YOffset, x.Name))
					.Fill(files.Length, GraphicsElementJson.Zero)];

				Logger.Debug($"Didn't find sprites.json file, using existing GraphicsElement offsets with {offsets.Count} images");
			}

			// clear existing images
			Logger.Debug("Clearing current images");
			GroupedImageViewModels.Clear();

			// load files
			var currentGroup = 0;
			var currentGroupIndex = 0;
			foreach (var (offset, i) in offsets.Select((x, i) => (x, i)))
			{
				var is1Pixel = string.IsNullOrEmpty(offset.Path);
				var img = is1Pixel ? OnePixelTransparent : Image.Load<Rgba32>(Path.Combine(directory, offset.Path));
				var newOffset = is1Pixel ? offset with { Flags = GraphicsElementFlags.HasTransparency } : offset;
				var graphicsElement = GraphicsElementFromImage(newOffset, img, PaletteMap);
				graphicsElement.Name = string.IsNullOrEmpty(graphicsElement.Name) ? DefaultImageTableNameProvider.GetImageName(i) : graphicsElement.Name;

				GroupedImageViewModels[currentGroup].Images.Add(new ImageViewModel(graphicsElement, PaletteMap));
				currentGroupIndex++;
				if (currentGroupIndex >= GroupedImageViewModels[currentGroup].Images.Count)
				{
					currentGroup++;
					currentGroupIndex = 0;
				}
			}
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
			ImageData = paletteMap.ConvertRgba32ImageToG1Data(img, flags),
			Name = ele.Name ?? string.Empty
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

		var offsets = new List<GraphicsElementJson>();

		foreach (var group in GroupedImageViewModels)
		{
			foreach (var image in group.Images)
			{
				var imageName = image.Name.Trim().ToLower().Replace(' ', '-');
				var groupName = group.GroupName.Trim().ToLower().Replace(' ', '-');
				var fileName = $"{groupName}_{imageName}.png";
				var path = Path.Combine(directory, fileName);
				await image.UnderlyingImage.SaveAsPngAsync(path);

				offsets.Add(new GraphicsElementJson(fileName, image.ToGraphicsElement()));
			}
		}

		var offsetsFile = Path.Combine(directory, "sprites.json");
		Logger.Info($"Saving sprite offsets to {offsetsFile}");
		await JsonFile.SerializeToFileAsync(offsets, offsetsFile);
	}
}

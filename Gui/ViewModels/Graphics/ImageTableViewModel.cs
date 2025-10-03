using Avalonia.Controls.Selection;
using Avalonia.Threading;
using Common;
using Common.Json;
using Common.Logging;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Common;
using Gui.ViewModels.LocoTypes.Objects.Building;
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
	public static ColourSwatch[] ColourSwatchesArr { get; } = Enum.GetValues<ColourSwatch>();

	[Reactive]
	public List<ColourRemapSwatchViewModel> ColourSwatches { get; init; }

	[Reactive]
	public ColourRemapSwatchViewModel SelectedPrimarySwatch { get; set; }

	[Reactive]
	public ColourRemapSwatchViewModel SelectedSecondarySwatch { get; set; }

	[Reactive]
	public ICommand ImportImagesCommand { get; set; }

	[Reactive]
	public ICommand ExportImagesCommand { get; set; }

	[Reactive]
	public ICommand ReplaceImageCommand { get; set; }
	[Reactive]
	public ICommand CropImageCommand { get; set; }
	[Reactive]
	public ICommand DeleteImageCommand { get; set; }
	[Reactive]
	public ICommand InsertImageAtCommand { get; set; }
	[Reactive]
	public ICommand AppendImageCommand { get; set; }

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

	public readonly ILogger Logger;

	[Reactive]
	public ObservableCollection<GroupedImageViewModel> GroupedImageViewModels { get; set; } = [];

	[Reactive]
	public ObservableCollection<ImageViewModel> LayeredImages { get; set; } = [];

	public BuildingComponentsViewModel? BuildingComponents { get; set; }

	ImageTable Model { get; init; }

	public ImageTableViewModel(ImageTable imageTable, ILogger logger, BuildingComponentsModel? buildingComponents = null)
	{
		ArgumentNullException.ThrowIfNull(imageTable);

		Model = imageTable;
		Logger = logger;
		RecreateViewModelGroupsFromImageTable(Model);

		// swatches/palettes
		ColourSwatches = [.. ColourSwatchesArr.Select(x => new ColourRemapSwatchViewModel()
		{
			Swatch = x,
			Colour = Model.PaletteMap.GetRemapSwatchFromName(x)[0].Color.ToAvaloniaColor(),
			GradientColours = [.. Model.PaletteMap.GetRemapSwatchFromName(x).Select(x => x.Color.ToAvaloniaColor())],
		})];

		SelectedPrimarySwatch = ColourSwatches.Single(x => x.Swatch == ColourSwatch.PrimaryRemap);
		SelectedSecondarySwatch = ColourSwatches.Single(x => x.Swatch == ColourSwatch.SecondaryRemap);

		// building components
		if (buildingComponents != null)
		{
			BuildingComponents = new(buildingComponents, imageTable);
		}

		_ = this.WhenAnyValue(o => o.SelectedPrimarySwatch).Skip(1)
			.Subscribe(_ => RecolourImages(SelectedPrimarySwatch.Swatch, SelectedSecondarySwatch.Swatch));
		_ = this.WhenAnyValue(o => o.SelectedSecondarySwatch).Skip(1)
			.Subscribe(_ => RecolourImages(SelectedPrimarySwatch.Swatch, SelectedSecondarySwatch.Swatch));
		_ = this.WhenAnyValue(o => o.AnimationSpeed)
			.Where(_ => animationTimer != null)
			.Subscribe(_ => animationTimer!.Interval = TimeSpan.FromMilliseconds(1000 / AnimationSpeed));

		ImportImagesCommand = ReactiveCommand.CreateFromTask(ImportImages);
		ExportImagesCommand = ReactiveCommand.CreateFromTask<bool>(ExportImages);
		ReplaceImageCommand = ReactiveCommand.CreateFromTask(ReplaceImage);
		CropImageCommand = ReactiveCommand.Create(CropImage);
		DeleteImageCommand = ReactiveCommand.CreateFromTask(DeleteImage);
		InsertImageAtCommand = ReactiveCommand.CreateFromTask<bool>(InsertImageAt);
		AppendImageCommand = ReactiveCommand.CreateFromTask<string>(AppendImage);
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

	private void RecreateViewModelGroupsFromImageTable(ImageTable imageTable)
	{
		// image tables
		GroupedImageViewModels.Clear();
		foreach (var group in imageTable.Groups)
		{
			var givm = new GroupedImageViewModel(group.Name, group.GraphicsElements.Select(ge => new ImageViewModel(ge)));
			givm.SelectionModel.SelectionChanged += SelectionChanged;
			GroupedImageViewModels.Add(givm);
		}
		this.RaisePropertyChanged(nameof(GroupedImageViewModels));
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
		// need to unselect the current selection
		var sm = (SelectionModel<ImageViewModel>)sender;
		if (SelectionModel != null && SelectionModel != sm)
		{
			SelectionModel.SelectionChanged -= SelectionChanged;
			SelectionModel.Clear();
			SelectionModel.SelectionChanged += SelectionChanged;
		}

		// set main selection to the new viewmodel selection
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

	public async Task ExportImages(bool prependGroupAndImageNameInFilename)
	{
		var folders = await PlatformSpecific.OpenFolderPicker();
		var dir = folders.FirstOrDefault();
		if (dir == null)
		{
			return;
		}

		var dirPath = dir.Path.LocalPath;
		await ExportImages(dirPath, prependGroupAndImageNameInFilename);
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

	public async Task DeleteImage()
	{
		if (SelectedImage == null)
		{
			return;
		}

		var index = SelectedImage.ImageTableIndex;
		Model.DeleteAt(index);

		RecreateViewModelGroupsFromImageTable(Model);
		await Task.CompletedTask;
	}

	private async Task AppendImage(string groupName)
	{
		var group = Model.Groups.FirstOrDefault(x => x.Name == groupName);

		if (group == null)
		{
			Logger.Error($"Couldn't find group named {groupName}");
			return;
		}

		// this doesn't work
		// because the groups don't store which indices they own, then
		// if a group is ever empty we can't ever add something into it with the correct index
		group.GraphicsElements.Add(ImageTableHelpers.GetErrorGraphicsElement(Model.Groups.Sum(x => x.GraphicsElements.Count)));
		RecreateViewModelGroupsFromImageTable(Model);
		await Task.CompletedTask;
	}

	private async Task InsertImageAt(bool insertBefore)
	{
		if (SelectedImage == null)
		{
			return;
		}

		var index = SelectedImage.ImageTableIndex;
		Model.InsertAt(index, insertBefore);

		RecreateViewModelGroupsFromImageTable(Model);
		await Task.CompletedTask;
	}

	// model stuff
	public void RecolourImages(ColourSwatch primary, ColourSwatch secondary)
	{
		foreach (var ivm in GroupedImageViewModels.SelectMany(x => x.Images))
		{
			ivm.RecolourImage(primary, secondary, Model.PaletteMap);
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

	async Task ImportImages(string directory)
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
					.Select((x, i) => new GraphicsElementJson(sanitised[i], x.XOffset, x.YOffset, x.Name))
					.Fill(files.Length, GraphicsElementJson.Zero)];

				Logger.Debug($"Didn't find sprites.json file, using existing GraphicsElement offsets with {offsets.Count} images");
			}

			var all = GroupedImageViewModels.SelectMany(x => x.Images).ToList();

			// load files
			var importedImages = new List<GraphicsElement>();
			foreach (var (offset, i) in offsets.Select((x, i) => (x, i)))
			{
				var is1Pixel = string.IsNullOrEmpty(offset.Path);
				var img = is1Pixel ? ImageTableHelpers.OnePixelTransparent : Image.Load<Rgba32>(Path.Combine(directory, offset.Path));
				var newOffset = is1Pixel ? offset with { Flags = GraphicsElementFlags.HasTransparency } : offset;
				var graphicsElement = GraphicsElementFromImage(newOffset, img, Model.PaletteMap, i);
				graphicsElement.Name = string.IsNullOrEmpty(graphicsElement.Name)
					? DefaultImageTableNameProvider.GetImageName(i)
					: graphicsElement.Name;

				importedImages.Add(graphicsElement);
			}

			foreach (var group in Model.Groups)
			{
				for (var i = 0; i < group.GraphicsElements.Count; i++)
				{
					var ge = group.GraphicsElements[i];
					if (ge.ImageTableIndex < 0 || ge.ImageTableIndex >= importedImages.Count)
					{
						Logger.Error($"Image[{ge.ImageTableIndex}] is out of range; only {importedImages.Count} were loaded. This graphics element will be set to empty.");
						group.GraphicsElements[i] = ImageTableHelpers.GetErrorGraphicsElement(ge.ImageTableIndex);
					}
					else
					{
						group.GraphicsElements[i] = importedImages[ge.ImageTableIndex];
					}
				}
			}

			// if there are any extras, add them to a new group
			var totalExistingElements = Model.Groups.Sum(x => x.GraphicsElements.Count);
			if (importedImages.Count > totalExistingElements)
			{
				var newGroup = new ImageTableGroup("<uncategorised-imported>", importedImages[totalExistingElements..]);
				Model.Groups.Add(newGroup);
			}

			RecreateViewModelGroupsFromImageTable(Model);
		}
		catch (Exception ex)
		{
			Logger.Error(ex);
		}
	}

	static GraphicsElement GraphicsElementFromImage(GraphicsElementJson ele, Image<Rgba32> img, PaletteMap paletteMap, int index)
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
			Name = ele.Name ?? string.Empty,
			Image = img,
			ImageTableIndex = index,
		};
	}

	async Task ExportImages(string directory, bool prependGroupAndImageNameInFilename)
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

		var invalidChars = Path.GetInvalidFileNameChars();

		foreach (var item in GroupedImageViewModels
			.SelectMany(group => group.Images, (group, image) => new { group.GroupName, Image = image })
			.OrderBy(x => x.Image.ImageTableIndex))
		{
			var image = item.Image;

			var fileName = $"{image.ImageTableIndex}.png";
			if (prependGroupAndImageNameInFilename)
			{
				var imageName = new string([.. item.Image.Name.ToLower().Replace(' ', '-').Where(x => !invalidChars.Contains(x))]).Trim();
				var groupName = new string([.. item.GroupName.ToLower().Replace(' ', '-').Where(x => !invalidChars.Contains(x))]).Trim();

				if (!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(imageName))
				{
					fileName = $"{groupName}_{imageName}.png";
				}
			}

			var path = Path.Combine(directory, fileName);
			await image.UnderlyingImage.SaveAsPngAsync(path);

			offsets.Add(new GraphicsElementJson(fileName, image.ToGraphicsElement(Model.PaletteMap)));
		}

		var offsetsFile = Path.Combine(directory, "sprites.json");
		Logger.Info($"Saving sprite offsets to {offsetsFile}");
		await JsonFile.SerializeToFileAsync(offsets, offsetsFile);
	}
}

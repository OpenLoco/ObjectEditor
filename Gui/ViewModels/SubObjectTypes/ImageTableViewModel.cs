using Avalonia.Controls.Selection;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Definitions.ObjectModels;
using DynamicData;
using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

	// what is displaying on the ui
	[Reactive]
	public ObservableCollection<ImageViewModel> ImageViewModels { get; set; } = [];

	[Reactive]
	public int SelectedImageIndex { get; set; } = -1;

	[Reactive]
	public SelectionModel<ImageViewModel> SelectionModel { get; set; }

	[Reactive]
	public ImageViewModel? SelectedImage { get; set; }

	readonly DispatcherTimer animationTimer;
	int currentFrameIndex;

	[Reactive]
	public IList<ImageViewModel> SelectedBitmaps { get; set; }

	[Reactive]
	public int AnimationSpeed { get; set; } = 40;

	ImageTableModel Model { get; init; }

	public ImageTableViewModel(ImageTableModel model)
	{
		Model = model;
		CreateSelectionModel();

		_ = this.WhenAnyValue(o => o.Model.Images)
			.Subscribe(_ => UpdateBitmaps());
		_ = this.WhenAnyValue(o => o.SelectedPrimarySwatch).Skip(1)
			.Subscribe(_ => UpdateBitmaps());
		_ = this.WhenAnyValue(o => o.SelectedSecondarySwatch).Skip(1)
			.Subscribe(_ => UpdateBitmaps());

		_ = this.WhenAnyValue(o => o.SelectedImageIndex)
			.Where(index => index >= 0 && index < ImageViewModels?.Count)
			.Subscribe(_ =>
			{
				// why the FUCK doesn't this trigged SelectedImage's property changed (and thus update the UI)
				SelectedImage = ImageViewModels[SelectedImageIndex];
				this.RaisePropertyChanged(nameof(SelectedImage));
			});

		_ = this.WhenAnyValue(o => o.AnimationSpeed)
			.Where(_ => animationTimer != null)
			.Subscribe(_ => animationTimer!.Interval = TimeSpan.FromMilliseconds(1000 / AnimationSpeed));

		ImportImagesCommand = ReactiveCommand.CreateFromTask(ImportImages);
		ExportImagesCommand = ReactiveCommand.CreateFromTask(ExportImages);
		ReplaceImageCommand = ReactiveCommand.CreateFromTask(ReplaceImage);
		CropAllImagesCommand = ReactiveCommand.Create(() =>
		{
			Model.CropAllImages(SelectedPrimarySwatch, SelectedSecondarySwatch);
			UpdateBitmaps();
		});

		UpdateBitmaps();

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

		// ... handle selection changed
		SelectedBitmaps = [.. sm.SelectedItems.Cast<ImageViewModel>()];
	}

	void AnimationTimer_Tick(object? sender, EventArgs e)
	{
		if (SelectionModel == null || SelectedBitmaps == null || SelectedBitmaps.Count == 0 || SelectionModel.SelectedIndexes.Count == 0)
		{
			return;
		}

		if (currentFrameIndex >= SelectedBitmaps.Count)
		{
			currentFrameIndex = 0;
		}

		// Update the displayed image viewmodel
		SelectedImageIndex = SelectionModel.SelectedIndexes[currentFrameIndex]; // disabling this also makes the memory leaks stop

		// Move to the next frame, looping back to the beginning if necessary
		currentFrameIndex = (currentFrameIndex + 1) % SelectedBitmaps.Count;
	}

	void CreateSelectionModel()
	{
		SelectionModel = new SelectionModel<ImageViewModel>
		{
			SingleSelect = false
		};
		SelectionModel.SelectionChanged += SelectionChanged;
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
			await Model.ImportImages(dirPath, SelectedPrimarySwatch, SelectedSecondarySwatch);
			UpdateBitmaps();
		}

		animationTimer.Start();
	}

	public void UpdateBitmaps()
	{
		Model.RecalcImages(SelectedPrimarySwatch, SelectedSecondarySwatch);
		var newImages = G1ImageConversion.CreateAvaloniaImages(Model.Images);

		ImageViewModels.Clear();
		var i = 0;
		foreach (var image in newImages)
		{
			ImageViewModels.Add(new ImageViewModel(i, Model.GetImageName(i), Model.G1Provider.GraphicsElements[i], image));
			i++;
		}
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
		await Model.ExportImages(dirPath);
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

		Model.UpdateImage(filename, SelectedImageIndex);
		UpdateBitmaps();
	}
}

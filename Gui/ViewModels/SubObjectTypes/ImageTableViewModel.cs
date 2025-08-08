using Avalonia.Controls.Selection;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using Dat;
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

public class ImageGroupViewModel : ReactiveObject
{
	public int InitialIndex { get; init; }
	public string GroupName { get; init; }
	public ObservableCollection<Bitmap?> Bitmaps { get; init; }

	[Reactive]
	public SelectionModel<Bitmap> SelectionModel { get; set; }

	[Reactive]
	public int SelectedImageIndex { get; set; } = -1;

	public IList<Bitmap> SelectedBitmaps { get; set; }

	ImageTableViewModel ParentViewModel { get; init; }

	ImageTableModel ParentModel { get; init; }

	public UIG1Element32? SelectedG1Element
		=> SelectedImageIndex == -1 || ParentModel.G1Provider.G1Elements.Count == 0 || SelectedImageIndex >= Bitmaps.Count || SelectedImageIndex + InitialIndex >= ParentModel.G1Provider.G1Elements.Count
		? null
		: new UIG1Element32(
			SelectedImageIndex,
			ParentModel.GetImageName(SelectedImageIndex),
			ParentModel.G1Provider.G1Elements[SelectedImageIndex + InitialIndex]);

	public Avalonia.Point SelectedG1ElementOffset
		=> SelectedG1Element == null
			? new Avalonia.Point()
			: new Avalonia.Point(-SelectedG1Element?.XOffset ?? 0, -SelectedG1Element?.YOffset ?? 0);
	public Avalonia.Size SelectedG1ElementSize
		=> SelectedG1Element == null
			? new Avalonia.Size()
			: new Avalonia.Size(SelectedG1Element?.Width ?? 0, SelectedG1Element?.Height ?? 0);

	[Reactive]
	public ICommand ReplaceImageCommand { get; set; }

	public ImageGroupViewModel(string groupName, ObservableCollection<Bitmap?> bitmaps, int initialIndex, ImageTableViewModel parentViewModel, ImageTableModel parentModel)
	{
		GroupName = groupName;
		Bitmaps = bitmaps;
		InitialIndex = initialIndex;
		ParentViewModel = parentViewModel;
		ParentModel = parentModel;

		CreateSelectionModel();

		_ = this.WhenAnyValue(o => o.SelectedImageIndex)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedG1Element)));
		_ = this.WhenAnyValue(o => o.SelectedG1Element)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedG1ElementOffset)));
		_ = this.WhenAnyValue(o => o.SelectedG1Element)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedG1ElementSize)));
		_ = this.WhenAnyValue(o => o.SelectedG1Element)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(ParentViewModel.SelectedBitmapPreview)));

		ReplaceImageCommand = ReactiveCommand.CreateFromTask(ReplaceImage);
	}

	void CreateSelectionModel()
	{
		SelectionModel = new SelectionModel<Bitmap>
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

	void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
	{
		var sm = (SelectionModel<Bitmap>)sender;

		if (sm.SelectedIndexes.Count > 0)
		{
			SelectedImageIndex = sm.SelectedIndex;
		}

		if (sm.SelectedItems.Count == 0)
		{
			return;
		}

		// ... handle selection changed
		SelectedBitmaps = [.. sm.SelectedItems.Cast<Bitmap>()];
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

		ParentModel.UpdateImage(filename, SelectedImageIndex);
		ParentModel.RecalcImages(ParentViewModel.SelectedPrimarySwatch, ParentViewModel.SelectedSecondarySwatch);
	}
}

public class ImageTableViewModel : ReactiveObject, IExtraContentViewModel
{
	public string Name => "Image Table";

	// used in the axaml to bind the combobox to the list of swatches
	public static ColourRemapSwatch[] ColourSwatchesArr { get; } = Enum.GetValues<ColourRemapSwatch>();

	[Reactive]
	public ColourRemapSwatch SelectedPrimarySwatch { get; set; } = ColourRemapSwatch.PrimaryRemap;

	[Reactive]
	public ColourRemapSwatch SelectedSecondarySwatch { get; set; } = ColourRemapSwatch.SecondaryRemap;

	readonly DispatcherTimer animationTimer;
	int currentFrameIndex;

	[Reactive]
	public int AnimationSpeed { get; set; } = 40;

	[Reactive]
	public ICommand ImportImagesCommand { get; set; }

	[Reactive]
	public ICommand ExportImagesCommand { get; set; }

	[Reactive]
	public ICommand CropAllImagesCommand { get; set; }

	[Reactive]
	public Bitmap SelectedBitmapPreview { get; set; }

	public Avalonia.Size SelectedBitmapPreviewBorder
		=> SelectedBitmapPreview == null
			? new Avalonia.Size()
			: new Avalonia.Size(SelectedBitmapPreview.Size.Width + 2, SelectedBitmapPreview.Size.Height + 2);

	[Reactive]
	public ObservableCollection<ImageGroupViewModel> BitmapGroups { get; set; }

	//[Reactive]
	public ImageGroupViewModel? SelectedGroup { get; set; } // todo: remove

	ImageTableModel Model { get; init; }

	public ImageTableViewModel(ImageTableModel model)
	{
		Model = model;

		_ = this.WhenAnyValue(o => o.Model.Images)
			.Subscribe(_ => UpdateBitmaps());
		_ = this.WhenAnyValue(o => o.SelectedPrimarySwatch).Skip(1)
			.Subscribe(_ => UpdateBitmaps());
		_ = this.WhenAnyValue(o => o.SelectedSecondarySwatch).Skip(1)
			.Subscribe(_ => UpdateBitmaps());

		_ = this.WhenAnyValue(o => o.SelectedBitmapPreview)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedBitmapPreviewBorder)));

		_ = this.WhenAnyValue(o => o.AnimationSpeed)
			.Subscribe(_ => UpdateAnimationSpeed());

		ImportImagesCommand = ReactiveCommand.CreateFromTask(ImportImages);
		ExportImagesCommand = ReactiveCommand.CreateFromTask(ExportImages);
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

	void UpdateAnimationSpeed()
	{
		if (animationTimer == null)
		{
			return;
		}

		animationTimer.Interval = TimeSpan.FromMilliseconds(1000 / AnimationSpeed);
	}

	void AnimationTimer_Tick(object? sender, EventArgs e)
	{
		var firstBitmapGroupWithValidSelection = BitmapGroups.FirstOrDefault(x => x.SelectionModel != null);
		if (firstBitmapGroupWithValidSelection == null)
		{
			return;
		}

		var selectionModel = firstBitmapGroupWithValidSelection.SelectionModel;
		if (selectionModel == null || selectionModel.SelectedItems.Count == 0)
		{
			return;
		}

		if (currentFrameIndex >= firstBitmapGroupWithValidSelection.SelectedBitmaps.Count)
		{
			currentFrameIndex = 0;
		}

		// Update the displayed image
		SelectedBitmapPreview = firstBitmapGroupWithValidSelection.SelectedBitmaps[currentFrameIndex];
		firstBitmapGroupWithValidSelection.SelectedImageIndex = selectionModel.SelectedIndexes[currentFrameIndex];

		// Move to the next frame, looping back to the beginning if necessary
		currentFrameIndex = (currentFrameIndex + 1) % firstBitmapGroupWithValidSelection.SelectedBitmaps.Count;
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

		var obs = Model.G1Provider.G1Elements.Select((x, i) => (i, x));
		BitmapGroups = [new ImageGroupViewModel("<default>", [.. G1ImageConversion.CreateAvaloniaImages(Model.Images)], 0, this, Model)];
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
}

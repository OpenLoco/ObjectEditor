using AvaGui.Models;
using Avalonia.Controls.Selection;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using OpenLoco.Dat;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Image = SixLabors.ImageSharp.Image;

namespace AvaGui.ViewModels
{
	public class ImageTableViewModel : ReactiveObject, IExtraContentViewModel, ILocoFileViewModel
	{
		readonly IHasG1Elements G1Provider;
		readonly IImageTableNameProvider NameProvider;

		public ImageTableViewModel(IHasG1Elements g1ElementProvider, IImageTableNameProvider imageNameProvider, PaletteMap paletteMap, IList<Image<Rgba32>> images)
		{
			G1Provider = g1ElementProvider;
			NameProvider = imageNameProvider;
			PaletteMap = paletteMap;
			Images = images;

			_ = this.WhenAnyValue(o => o.G1Provider)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.PaletteMap)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.Zoom)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.SelectedImageIndex)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedG1Element)));
			_ = this.WhenAnyValue(o => o.Images)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.AnimationSpeed)
				.Subscribe(_ => UpdateAnimationSpeed());

			ImportImagesCommand = ReactiveCommand.Create(ImportImages);
			ExportImagesCommand = ReactiveCommand.Create(ExportImages);

			SelectionModel = new SelectionModel<Bitmap>
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

		readonly DispatcherTimer animationTimer;
		int currentFrameIndex;

		public IList<Bitmap> SelectedBitmaps { get; set; }

		[Reactive]
		public Bitmap SelectedBitmapPreview { get; set; }

		[Reactive]
		public int AnimationWindowHeight { get; set; }

		void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
		{
			var sm = (SelectionModel<Bitmap>)sender;

			if (sm.SelectedIndexes.Count > 0)
			{
				SelectedImageIndex = sm.SelectedIndex;
			}

			// ... handle selection changed
			SelectedBitmaps = sm.SelectedItems.Cast<Bitmap>().ToList();
			AnimationWindowHeight = (int)SelectedBitmaps.Max(x => x.Size.Height) * 2;
		}

		[Reactive]
		public int AnimationSpeed { get; set; } = 40;

		void UpdateAnimationSpeed()
		{
			if (animationTimer == null)
			{
				return;
			}

			animationTimer.Interval = TimeSpan.FromMilliseconds(1000 / AnimationSpeed);
		}

		private void AnimationTimer_Tick(object? sender, EventArgs e)
		{
			if (SelectedBitmaps == null || SelectedBitmaps.Count == 0)
			{
				return;
			}

			if (currentFrameIndex >= SelectedBitmaps.Count)
			{
				currentFrameIndex = 0;
			}

			// Update the displayed image
			SelectedBitmapPreview = SelectedBitmaps[currentFrameIndex];
			SelectedImageIndex = SelectionModel.SelectedIndexes[currentFrameIndex];

			// Move to the next frame, looping back to the beginning if necessary
			currentFrameIndex = (currentFrameIndex + 1) % SelectedBitmaps.Count;
		}

		[Reactive]
		public PaletteMap PaletteMap { get; set; }

		[Reactive]
		public ICommand ImportImagesCommand { get; set; }

		[Reactive]
		public ICommand ExportImagesCommand { get; set; }

		[Reactive]
		public int Zoom { get; set; } = 1;

		// where the actual image data is stored
		[Reactive]
		public IList<Image<Rgba32>> Images { get; set; }

		// what is displaying on the ui
		public IList<Bitmap?> Bitmaps
		{
			get
			{
				// this shenanigans is to handle the DuplicatePrevious flag. we store it as null
				// and here we just reuse the previous image if the flag is set (ie null image)
				var list = G1ImageConversion.CreateAvaloniaImages(Images).ToList();
				Bitmap? prevValue = null;

				for (var i = 0; i < list.Count; i++)
				{
					if (list[i] == null)
					{
						list[i] = prevValue;
					}
					else
					{
						prevValue = list[i];
					}
				}
				return list;
			}
		}

		[Reactive]
		public int SelectedImageIndex { get; set; } = -1;

		[Reactive]
		public SelectionModel<Bitmap> SelectionModel { get; set; }

		public UIG1Element32? SelectedG1Element
			=> SelectedImageIndex == -1 || G1Provider.G1Elements.Count == 0
			? null
			: new UIG1Element32(SelectedImageIndex, GetImageName(NameProvider, SelectedImageIndex), G1Provider.G1Elements[SelectedImageIndex]);

		public async Task ImportImages()
		{
			var folders = await PlatformSpecific.OpenFolderPicker();
			var dir = folders.FirstOrDefault();
			if (dir == null)
			{
				return;
			}

			var dirPath = dir.Path.LocalPath;
			if (Directory.Exists(dirPath) && Directory.EnumerateFiles(dirPath).Any())
			{
				var files = Directory.GetFiles(dirPath);
				var sorted = files.OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('-')[0]));

				var g1Elements = new List<G1Element32>();
				var i = 0;
				foreach (var file in sorted)
				{
					var img = Image.Load<Rgba32>(file);
					Images[i] = img;
					var currG1 = G1Provider.G1Elements[i++];
					currG1.ImageData = PaletteMap.ConvertRgba32ImageToG1Data(img, currG1.Flags); // simply overwrite existing pixel data
				}
			}

			this.RaisePropertyChanged(nameof(Bitmaps));
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
			if (Directory.Exists(dirPath))
			{
				var counter = 0;
				foreach (var image in Images)
				{
					var imageName = counter++.ToString(); // todo: use GetImageName from winforms project
					var path = Path.Combine(dir.Path.LocalPath, $"{imageName}.png");
					//logger.Debug($"Saving image to {path}");
					await image.SaveAsPngAsync(path);
				}
			}
		}

		public static string GetImageName(IImageTableNameProvider nameProvider, int counter)
			=> nameProvider.TryGetImageName(counter, out var value) && !string.IsNullOrEmpty(value)
				? value
				: counter.ToString();
	}
}

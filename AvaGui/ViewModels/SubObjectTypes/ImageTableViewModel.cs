using AvaGui.Models;
using Avalonia.Controls.Selection;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.WebRequestMethods;
using Image = SixLabors.ImageSharp.Image;

namespace AvaGui.ViewModels
{
	public record SpriteOffset(
		[property: JsonPropertyName("path")] string Path,
		[property: JsonPropertyName("x")] int16_t X,
		[property: JsonPropertyName("y")] int16_t Y);

	public class ImageTableViewModel : ReactiveObject, IExtraContentViewModel, ILocoFileViewModel
	{
		readonly IHasG1Elements G1Provider;
		readonly IImageTableNameProvider NameProvider;
		readonly ILogger Logger;

		public ImageTableViewModel(IHasG1Elements g1ElementProvider, IImageTableNameProvider imageNameProvider, PaletteMap paletteMap, IList<Image<Rgba32>> images, ILogger logger)
		{
			G1Provider = g1ElementProvider;
			NameProvider = imageNameProvider;
			PaletteMap = paletteMap;
			Images = images;
			Logger = logger;

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
			Logger = logger;
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

			if (sm.SelectedItems.Count == 0)
			{
				return;
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
			if (SelectedBitmaps == null || SelectedBitmaps.Count == 0 || SelectionModel.SelectedIndexes.Count == 0)
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

		//todo: second half should be model
		public async Task ImportImages()
		{
			var folders = await PlatformSpecific.OpenFolderPicker();
			var dir = folders.FirstOrDefault();
			if (dir == null)
			{
				return;
			}

			var dirPath = dir.Path.LocalPath;
			if (!Directory.Exists(dirPath))
			{
				return;
			}

			var offsetsFile = Path.Combine(dirPath, "sprites.json");
			var spritesFolder = Path.Combine(dirPath, "sprites");
			if (File.Exists(offsetsFile) && Directory.Exists(spritesFolder))
			{
				// found blender folder
				var offsets = JsonSerializer.Deserialize<IEnumerable<SpriteOffset>>(offsetsFile);
				LoadSpritesFolderWithOffsets(dirPath, offsets.ToList());

			}
			else
			{
				LoadSpritesFolder(dirPath);
			}
		}

		private void LoadSpritesFolderWithOffsets(string dirPath, IList<SpriteOffset> offsets)
		{
			var files = new List<string>();
			foreach (var spriteOffset in offsets)
			{
				var filename = Path.Combine(dirPath, spriteOffset.Path);
				if (!Path.Exists(filename))
				{
					throw new ArgumentOutOfRangeException(nameof(spriteOffset.Path), $"File listed in sprites.json doesn't exist: \"{filename}\"");
				}
				files.Add(filename);
			}

			var sorted = files.OrderBy(static f =>
			{
				var match = Regex.Match(Path.GetFileNameWithoutExtension(f), @".*?(\d+).*?");
				return match.Success
					? int.Parse(match.Groups[1].Value)
					: throw new InvalidDataException($"Directory contains file that doesn't contain a number={f}");
			});

			try
			{
				var sorted = files.OrderBy(static f =>
				{
					var match = Regex.Match(Path.GetFileNameWithoutExtension(f), @".*?(\d+).*?");
					return match.Success
						? int.Parse(match.Groups[1].Value)
						: throw new InvalidDataException($"Directory contains file that doesn't contain a number={f}");
				});

				var sortedH = sorted.ToImmutableHashSet();

				var g1Elements = new List<G1Element32>();
				var i = 0;
				var zeroOffset = new SpriteOffset(string.Empty, 0, 0);
				foreach (var file in sorted)
				{
					var img = Image.Load<Rgba32>(file);
					var offset = offsets == null ? zeroOffset : offsets[i];
					Images[i] = img;
					var currG1 = G1Provider.G1Elements[i];
					currG1 = currG1 with
					{
						Width = (int16_t)img.Width,
						Height = (int16_t)img.Height,
						Flags = currG1.Flags & ~G1ElementFlags.IsRLECompressed, // SawyerStreamWriter::SaveImageTable does this anyways
						ImageData = PaletteMap.ConvertRgba32ImageToG1Data(img, currG1.Flags),
						XOffset = offset.X,
						YOffset = offset.Y,
					};
					G1Provider.G1Elements[i] = currG1;
					i++;
				}

				this.RaisePropertyChanged(nameof(Bitmaps));
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		private void LoadSpritesFolder(string dirPath)
		{
			var files = Directory.GetFiles(dirPath);
			if (files.Length == 0)
			{
				return;
			}

			try
			{
				var sorted = files.OrderBy(static f =>
				{
					var match = Regex.Match(Path.GetFileNameWithoutExtension(f), @".*?(\d+).*?");
					return match.Success
						? int.Parse(match.Groups[1].Value)
						: throw new InvalidDataException($"Directory contains file that doesn't contain a number={f}");
				});

				var sortedH = sorted.ToImmutableHashSet();

				var g1Elements = new List<G1Element32>();
				var i = 0;
				var zeroOffset = new SpriteOffset(string.Empty, 0, 0);
				foreach (var file in sorted)
				{
					var img = Image.Load<Rgba32>(file);
					Images[i] = img;
					var currG1 = G1Provider.G1Elements[i];
					currG1 = currG1 with
					{
						Width = (int16_t)img.Width,
						Height = (int16_t)img.Height,
						Flags = currG1.Flags & ~G1ElementFlags.IsRLECompressed, // SawyerStreamWriter::SaveImageTable does this anyways
						ImageData = PaletteMap.ConvertRgba32ImageToG1Data(img, currG1.Flags),
					};
					G1Provider.G1Elements[i] = currG1;
					i++;
				}

				this.RaisePropertyChanged(nameof(Bitmaps));
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		// todo: second half should be in model
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

using Avalonia.Media.Imaging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types.SCV5;
using OpenLoco.Gui.Models;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class SCV5ViewModel : BaseLocoFileViewModel
	{
		public SCV5ViewModel(FileSystemItemBase currentFile, ObjectEditorModel model)
			: base(currentFile, model)
			=> Load();

		[Reactive]
		public S5File? CurrentS5File { get; set; }

		[Reactive]
		public ObservableCollection<S5HeaderViewModel>? RequiredObjects { get; set; }

		[Reactive]
		public ObservableCollection<S5HeaderViewModel>? PackedObjects { get; set; }

		[Reactive]
		public WriteableBitmap? Map { get; set; }

		[Reactive]
		public Dictionary<ElementType, Bitmap> Maps { get; set; }

		[Reactive, Range(0, Limits.kMapColumns - 1)]
		public int TileElementX { get; set; }

		[Reactive, Range(0, Limits.kMapRows - 1)]
		public int TileElementY { get; set; }

		public ObservableCollection<TileElement> CurrentTileElements
			=> CurrentS5File?.TileElementMap != null && TileElementX >= 0 && TileElementX < 384 && TileElementY >= 0 && TileElementY < 384
				? [.. CurrentS5File.TileElementMap[TileElementX, TileElementY]]
				: [];

		public override void Load()
		{
			logger?.Info($"Loading scenario from {CurrentFile.Filename}");
			CurrentS5File = SawyerStreamReader.LoadSave(CurrentFile.Filename, Model.Logger);

			if (CurrentS5File == null)
			{
				logger?.Error($"Unable to load {CurrentFile.Filename}");
				return;
			}

			RequiredObjects = new ObservableCollection<S5HeaderViewModel>([.. CurrentS5File.RequiredObjects.Where(x => x.Checksum != 0).Select(x => new S5HeaderViewModel(x)).OrderBy(x => x.Name)]);
			PackedObjects = new ObservableCollection<S5HeaderViewModel>([.. CurrentS5File.PackedObjects.ConvertAll(x => new S5HeaderViewModel(x.Item1)).OrderBy(x => x.Name)]); // note: cannot bind to this, but it'll allow us to display at least

			_ = this.WhenAnyValue(o => o.TileElementX)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentTileElements)));

			_ = this.WhenAnyValue(o => o.TileElementY)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentTileElements)));

			if (CurrentS5File.TileElementMap != null)
			{
				try
				{
					DrawMap();
				}
				catch (Exception ex)
				{
					logger?.Error(ex);
				}
			}
		}

		void DrawMap()
		{
			Map = new WriteableBitmap(new Avalonia.PixelSize(384, 384), new Avalonia.Vector(92, 92), Avalonia.Platform.PixelFormat.Rgba8888);
			using (var fb = Map.Lock())
			{
				var teMap = CurrentS5File!.TileElementMap!;
				for (var y = 0; y < teMap.GetLength(1); ++y)
				{
					for (var x = 0; x < teMap.GetLength(0); ++x)
					{
						var el = teMap[x, y].Last();
						unsafe
						{
							var rgba = (byte*)fb.Address;
							var idx = ((x * 384) + y) * 4; // not sure why this has to be reversed to match loco

							if (el.Type == ElementType.Surface)
							{
								var els = el as SurfaceElement;
								if (els!.IsWater())
								{
									rgba[idx + 0] = (byte)(74 + el.BaseZ);
									rgba[idx + 1] = (byte)(118 + el.BaseZ);
									rgba[idx + 2] = (byte)(124 + el.BaseZ);
								}
								else
								{
									rgba[idx + 0] = (byte)(111 + el.BaseZ - (els.TerrainType() * 8));
									rgba[idx + 1] = (byte)(75 + el.BaseZ + (els.TerrainType() * 8));
									rgba[idx + 2] = (byte)(23 + el.BaseZ + (els.TerrainType() * 8));

									//rgbaValues[idx + 0] = (byte)(el.Terrain() == 1 ? 255 : 0);
									//rgbaValues[idx + 1] = (byte)(el.Terrain() == 1 ? 255 : 0);
									//rgbaValues[idx + 2] = (byte)(el.Terrain() == 1 ? 255 : 0);
								}
							}
							else if (el.Type == ElementType.Track)
							{
								rgba[idx + 0] = 131;
								rgba[idx + 1] = 151;
								rgba[idx + 2] = 151;
							}
							else if (el.Type == ElementType.Station)
							{
								rgba[idx + 0] = 255;
								rgba[idx + 1] = 163;
								rgba[idx + 2] = 79;
							}
							else if (el.Type == ElementType.Signal)
							{
								rgba[idx] = 255;
								rgba[idx + 1] = 0;
								rgba[idx + 2] = 0;
							}
							else if (el.Type == ElementType.Building)
							{
								rgba[idx] = 179;
								rgba[idx + 1] = 79;
								rgba[idx + 2] = 79;
							}
							else if (el.Type == ElementType.Tree)
							{
								rgba[idx] = 71;
								rgba[idx + 1] = 175;
								rgba[idx + 2] = 39;
							}
							else if (el.Type == ElementType.Wall)
							{
								rgba[idx] = 200;
								rgba[idx + 1] = 200;
								rgba[idx + 2] = 0;
							}
							else if (el.Type == ElementType.Road)
							{
								rgba[idx] = 47;
								rgba[idx + 1] = 67;
								rgba[idx + 2] = 67;
							}
							else if (el.Type == ElementType.Industry)
							{
								rgba[idx] = 139;
								rgba[idx + 1] = 139;
								rgba[idx + 2] = 191;
							}

							rgba[idx + 3] = 255;
						}
					}
				}
			}
		}

		public override void Save() => logger?.Warning("Save is not currently implemented");

		public override void SaveAs() => logger?.Warning("SaveAs is not currently implemented");

		//public override void Save()
		//	=> Save(CurrentFile.Filename);

		//public override void SaveAs()
		//{
		//	var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.SCV5FileTypes)).Result;
		//	if (saveFile == null)
		//	{
		//		return;
		//	}

		//	Save(saveFile.Path.LocalPath);
		//}

		//void Save(string filename)
		//{
		//	logger?.Info($"Saving scenario/save/landscape to {filename}");

		//	var newFile = CurrentS5File with
		//	{
		//		RequiredObjects = [.. RequiredObjects.Select(x => x.GetAsUnderlyingType())],
		//	};

		//	var bytes = newFile.Write();
		//	File.WriteAllBytes(filename, bytes);
		//}
	}
}

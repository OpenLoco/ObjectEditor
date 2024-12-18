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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class SCV5ViewModel : BaseLocoFileViewModel
	{
		public SCV5ViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		[Reactive]
		public S5File? CurrentS5File { get; set; }

		[Reactive]
		public BindingList<S5HeaderViewModel>? RequiredObjects { get; set; }

		[Reactive]
		public BindingList<S5HeaderViewModel>? PackedObjects { get; set; }

		[Reactive]
		public BindingList<TileElement>? TileElements { get; set; }


		public List<TileElement>[,] TileElementMap { get; set; }

		//[Reactive]
		//public Image<Rgba32> Map { get; set; }

		[Reactive]
		public WriteableBitmap Map { get; set; }

		[Reactive]
		public Dictionary<ElementType, Bitmap> Maps { get; set; }

		[Reactive, Range(0, Limits.kMapColumns - 1)]
		public int TileElementX { get; set; }

		[Reactive, Range(0, Limits.kMapRows - 1)]
		public int TileElementY { get; set; }

		public BindingList<TileElement> CurrentTileElements
		{
			get
			{
				if (TileElementX >= 0 && TileElementX < 384 && TileElementY >= 0 && TileElementY < 384)
				{
					return TileElementMap[TileElementX, TileElementY].ToBindingList();
				}
				return [];
			}
		}

		public override void Load()
		{
			Logger?.Info($"Loading scenario from {CurrentFile.Filename}");
			CurrentS5File = SawyerStreamReader.LoadSave(CurrentFile.Filename, Model.Logger);
			RequiredObjects = new BindingList<S5HeaderViewModel>(CurrentS5File!.RequiredObjects.ConvertAll(x => new S5HeaderViewModel(x)));
			PackedObjects = new BindingList<S5HeaderViewModel>(CurrentS5File!.PackedObjects.ConvertAll(x => new S5HeaderViewModel(x.Item1))); // note: cannot bind to this, but it'll allow us to display at least
			TileElements = new BindingList<TileElement>(CurrentS5File!.TileElements);

			_ = this.WhenAnyValue(o => o.TileElementX)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentTileElements)));

			_ = this.WhenAnyValue(o => o.TileElementY)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentTileElements)));

			//Map = new Image<Rgba32>(384, 384);

			// todo: find a way to actually render this in the UI. code here works fine, just dunno the ui
			TileElementMap = new List<TileElement>[Limits.kMapColumns, Limits.kMapRows];

			var i = 0;
			Map = new WriteableBitmap(new Avalonia.PixelSize(384, 384), new Avalonia.Vector(92, 92), Avalonia.Platform.PixelFormat.Rgba8888);
			using (var fb = Map.Lock())
			{
				for (var y = 0; y < TileElementMap.GetLength(1); ++y)
				{
					for (var x = 0; x < TileElementMap.GetLength(0); ++x)
					{
						TileElementMap[x, y] = [];
						do
						{
							TileElementMap[x, y].Add(CurrentS5File!.TileElements[i]);

							var el = TileElementMap[x, y].Last();
							unsafe
							{
								var rgba = (byte*)fb.Address;
								var idx = ((x * 384) + y) * 4; // not sure why this has to be reversed to match loco

								if (el.Type == ElementType.Surface)
								{
									if (el.IsWater())
									{
										rgba[idx + 0] = (byte)(74 + el.BaseZ);
										rgba[idx + 1] = (byte)(118 + el.BaseZ);
										rgba[idx + 2] = (byte)(124 + el.BaseZ);
									}
									else
									{
										rgba[idx + 0] = (byte)(111 + el.BaseZ - (el.Terrain() * 8));
										rgba[idx + 1] = (byte)(75 + el.BaseZ + (el.Terrain() * 8));
										rgba[idx + 2] = (byte)(23 + el.BaseZ + (el.Terrain() * 8));

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


							i++;
						}
						while (!CurrentS5File!.TileElements[i - 1].IsLast());
					}
				}
			}

			//TileElements = new BindingList<TileElement>(CurrentS5File!.TileElements);
		}

		public override void Save() => Logger?.Error("Saving SC5/SV5 is not implemented yet");

		public override void SaveAs() => Logger?.Error("Saving SC5/SV5 is not implemented yet");
	}
}

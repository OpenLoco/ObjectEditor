using Avalonia.Media.Imaging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types.SCV5;
using OpenLoco.Definitions.Database;
using OpenLoco.Gui.Models;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class SCV5ViewModel : BaseLocoFileViewModel
	{
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

		[Reactive]
		public GameObjDataFolder LastGameObjDataFolder { get; set; } = GameObjDataFolder.Locomotion;
		public ReactiveCommand<GameObjDataFolder, Unit> DownloadMissingObjectsToGameObjDataCommand { get; }

		public SCV5ViewModel(FileSystemItemBase currentFile, ObjectEditorModel model)
			: base(currentFile, model)
		{
			Load();

			DownloadMissingObjectsToGameObjDataCommand = ReactiveCommand.CreateFromTask<GameObjDataFolder, Unit>(async targetFolder =>
			{
				var folder = model.Settings.GetGameObjDataFolder(targetFolder);
				if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
				{
					logger.Error($"The specified [{targetFolder}] ObjData directory is invalid: \"{folder}\"");
					return default;
				}

				LastGameObjDataFolder = targetFolder;

				if (CurrentS5File == null)
				{
					logger.Error("Current S5File is null");
					return default;
				}

				if (model.ObjectServiceClient == null)
				{
					logger.Error("The object service client is null");
					return default;
				}

				var gameFolderIndex = ObjectIndex.LoadOrCreateIndex(folder, logger);

				if (model.ObjectIndexOnline == null)
				{
					// need to download the index, ie call /objects/list
					logger.Info("Online index doesn't exist - downloading now");

					model.ObjectIndexOnline = new ObjectIndex((await Model.ObjectServiceClient.GetObjectListAsync())
						.Select(x => new ObjectIndexEntry(x.Id.ToString(), x.DisplayName, x.DatChecksum, null, x.InternalName, x.ObjectType, x.ObjectSource, x.CreatedDate, x.ModifiedDate, x.VehicleType)));

					logger.Info("Index downloaded");
					// technically should check if the index is downloaded and valid now
				}

				foreach (var obj in CurrentS5File.RequiredObjects)
				{
					if (gameFolderIndex.Objects.Contains(x => x.DisplayName == obj.Name && x.DatChecksum == obj.Checksum))
					{
						continue;
					}

					// obj is missing - we need to download
					logger.Info($"Scenario {currentFile.DisplayName} has missing {obj.ObjectType} \"{obj.Name}\" with checksum {obj.Checksum}");

					var onlineObj = model.ObjectIndexOnline
						.Objects
						.FirstOrDefault(x => x.DisplayName == obj.Name && x.DatChecksum == obj.Checksum); // ideally would be SingleOrDefault but unfortunately DAT is not unique

					if (onlineObj == null)
					{
						logger.Error("Couldn't find a matching object in the online index");
						continue;
					}

					if (onlineObj.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
					{
						logger.Warning("This is a vanilla object. Cannot download from Object Service - your base game installation may be corrupt");
						continue;
					}

					if (!int.TryParse(onlineObj.Filename, out var id))
					{
						// couldn't get the id from the name, which is set into Filename. see FolderTreeViewModel::LoadOnlineDirectoryAsync() for more details
						logger.Error("Couldn't get object id from its filename");
						continue;
					}

					// download actual file
					var downloadedObjBytes = await model.ObjectServiceClient.GetObjectFileAsync(id);

					if (downloadedObjBytes == null)
					{
						logger.Error("Downloaded bytes was null");
						continue;
					}

					// write file to the selected directory
					var filename = $"{Path.Combine(folder, onlineObj.DisplayName ?? onlineObj.InternalName ?? onlineObj.Filename)}.dat";

					if (File.Exists(filename))
					{
						logger.Warning($"{filename} already exists - will NOT overwrite it");
						continue;
					}

					logger.Info($"Writing file to {filename}");

					await File.WriteAllBytesAsync(filename, downloadedObjBytes);
				}

				return default;
			});
		}

		void DownloadMissingObjectsToGameFolder(GameObjDataFolder targetFolder)
		{

		}

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

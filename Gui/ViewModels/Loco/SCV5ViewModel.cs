using Avalonia.Media.Imaging;
using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types.SCV5;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Gui.Models;
using Definitions;
using Microsoft.Extensions.Logging;
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
using System.Threading.Tasks;
using ObjectService.Services;

namespace Gui.ViewModels;

public class SCV5ViewModel : BaseFileViewModel<S5File>
{
	//[Reactive]
	//public S5File? Model { get; set; }

	public RequiredObjectsListViewModel RequiredObjects { get; }

	[Reactive]
	public ObservableCollection<ObjectModelHeaderViewModel>? PackedObjects { get; set; }

	[Reactive]
	public WriteableBitmap? Map { get; set; }

	[Reactive]
	public Dictionary<ElementType, Bitmap> Maps { get; set; } = [];

	[Reactive, Range(0, Limits.kMapColumnsVanilla - 1)]
	public int TileElementX { get; set; }

	[Reactive, Range(0, Limits.kMapRowsVanilla - 1)]
	public int TileElementY { get; set; }

	public ObservableCollection<TileElement> CurrentTileElements
		=> Model?.TileElementMap != null && TileElementX >= 0 && TileElementX < Model.GetMapSize().Width && TileElementY >= 0 && TileElementY < Model.GetMapSize().Height
			? [.. Model.TileElementMap[TileElementX, TileElementY]]
			: [];

	[Reactive]
	public GameObjDataFolder LastGameObjDataFolder { get; set; } = GameObjDataFolder.LocomotionSteam;
	public ReactiveCommand<GameObjDataFolder, Unit> DownloadMissingObjectsToGameObjDataCommand { get; }

	public SCV5ViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
	{
		RequiredObjects = new RequiredObjectsListViewModel(editorContext);
		SaveIsVisible = false;
		SaveAsIsVisible = false;
		Load();
		DownloadMissingObjectsToGameObjDataCommand = ReactiveCommand.CreateFromTask<GameObjDataFolder>(DownloadMissingObjects);
	}

	public override void Load()
	{
		Logger.LogInformation("Loading scenario from {FileName}", CurrentFile.FileName);
		if (CurrentFile.FileName == null)
		{
			Logger.LogError("Scenario file name was null");
			return;
		}

		Model = SawyerStreamReader.LoadSave(CurrentFile.FileName, EditorContext.Logger)!;

		if (Model == null)
		{
			Logger.LogError("Unable to load {FileName}", CurrentFile.FileName);
			return;
		}

		var headers = Model.RequiredObjects
			.Where(x => x.Checksum != 0)
			.Select(x => x.Convert());
		RequiredObjects.Replace(headers);

		var po = Model.PackedObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x.Item1.Convert())).OrderBy(x => x.Name);
		PackedObjects = [with([.. po])];

		_ = this.WhenAnyValue(o => o.TileElementX)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentTileElements)));

		_ = this.WhenAnyValue(o => o.TileElementY)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentTileElements)));

		if (Model.TileElementMap != null)
		{
			try
			{
				DrawMap();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to draw map for scenario \"{FileName}\"", CurrentFile.FileName);
			}
		}
	}

	async Task DownloadMissingObjects(GameObjDataFolder targetFolder)
	{
		var folder = EditorContext.Settings.GetGameObjDataFolder(targetFolder);

		if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
		{
			Logger.LogError("The specified [{TargetFolder}] ObjData directory is invalid: \"{Folder}\"", targetFolder, folder);
			return;
		}

		LastGameObjDataFolder = targetFolder;

		if (Model == null)
		{
			Logger.LogError("Current S5File is null");
			return;
		}

		var onlineClient = EditorContext.RemoteObjectServiceClient;
		if (onlineClient == null)
		{
			Logger.LogError("The remote object service client is null");
			return;
		}

		var gameFolderScan = await DatFolderScanner.ScanDirectoryAsync(folder, Logger).ConfigureAwait(true);
		var gameFolderIndex = new ObjectIndex(gameFolderScan.Succeeded.Select(r => new ObjectIndexEntry(
			r.DatName, r.RelativePath, null, r.DatChecksum, r.xxHash3,
			r.ObjectType, r.ObjectSource, r.CreatedDate, r.ModifiedDate, r.VehicleType)));

		var onlineIndex = EditorContext.GetObjectIndex(onlineClient, FileLocation.Online);
		if (onlineIndex == null)
		{
			// need to download the index, ie call /objects/list
			Logger.LogInformation("Online index doesn't exist - downloading now");

			onlineIndex = new ObjectIndex((await onlineClient.GetObjectListAsync())
				.Select(x => new ObjectIndexEntry(x.DisplayName, null, x.Id, x.DatChecksum, null, x.ObjectType, x.ObjectSource, x.CreatedDate, x.ModifiedDate, x.VehicleType, x.Availability)));
			EditorContext.SetObjectIndex(onlineClient, FileLocation.Online, onlineIndex);

			Logger.LogInformation("Index downloaded");
			// technically should check if the index is downloaded and valid now
		}

		foreach (var obj in RequiredObjects.Items)
		{
			if (OriginalObjectFiles.GetFileSource(obj.Name, obj.DatChecksum, obj.ObjectSource.Convert()) is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
			{
				continue;
			}

			if (gameFolderIndex.Objects.Contains(x => x.DisplayName == obj.Name && x.DatChecksum == obj.DatChecksum))
			{
				continue;
			}

			// obj is missing - we need to download
			Logger.LogInformation("Scenario {DisplayName} has missing object. Name=\"{Name}\" Checksum={DatChecksum} ObjectType={ObjectType} ", CurrentFile.DisplayName, obj.Name, obj.DatChecksum, obj.ObjectType);

			var onlineObj = onlineIndex
				.Objects
				.FirstOrDefault(x => x.DisplayName == obj.Name && x.DatChecksum == obj.DatChecksum); // ideally would be SingleOrDefault but unfortunately DAT is not unique

			if (onlineObj == null)
			{
				Logger.LogError("Couldn't find a matching object in the online index. Name=\"{Name}\" Checksum={DatChecksum} ObjectType={ObjectType} ", obj.Name, obj.DatChecksum, obj.ObjectType);

				// Add this missing object to the server's missing objects list
				var missingEntry = new DtoObjectMissingPost(
					obj.Name,
					obj.DatChecksum,
					obj.ObjectType);

				var result = await onlineClient.AddMissingObjectAsync(missingEntry);
				if (result != null)
				{
					Logger.LogInformation("Successfully added missing object to server: Id={Id} Name=\"{Name}\" Checksum=({DatChecksum})", result.Id, obj.Name, obj.DatChecksum);
				}
				else
				{
					Logger.LogError("Failed to add missing object to server: Name=\"{Name}\" Checksum=({DatChecksum})", obj.Name, obj.DatChecksum);
				}

				continue;
			}

			if (onlineObj.Id == null)
			{
				Logger.LogError("Downloaded object had no Id - this is a problem with the server");
				continue;
			}

			// download actual file
			var downloadedObjBytes = await onlineClient.GetObjectFileAsync(onlineObj.Id.Value);

			if (downloadedObjBytes == null)
			{
				Logger.LogError("Downloaded bytes was null");
				continue;
			}

			// write file to the selected directory
			var filename = $"{onlineObj.DisplayName ?? onlineObj.FileName}-{onlineObj.Id}.dat";
			filename = Path.Combine(folder, filename);

			if (File.Exists(filename))
			{
				Logger.LogWarning("{Filename} already exists - will NOT overwrite it", filename);
				continue;
			}

			Logger.LogInformation("Writing file to {Filename}", filename);

			await File.WriteAllBytesAsync(filename, downloadedObjBytes);
		}

		return;
	}

	void DrawMap()
	{
		(var mapWidth, var mapHeight) = Model.GetMapSize();
		Map = new WriteableBitmap(new Avalonia.PixelSize(mapWidth, mapHeight), new Avalonia.Vector(92, 92), Avalonia.Platform.PixelFormat.Rgba8888);
		using (var fb = Map.Lock())
		{
			var teMap = Model!.TileElementMap!;
			for (var y = 0; y < teMap.GetLength(1); ++y)
			{
				for (var x = 0; x < teMap.GetLength(0); ++x)
				{
					var el = teMap[x, y].Last();
					unsafe
					{
						var rgba = (byte*)fb.Address;
						var idx = ((x * mapWidth) + y) * 4; // not sure why this has to be reversed to match loco

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

	public override void Save()
		=> Logger.LogWarning("Save is not currently implemented");

	public override Task<string?> SaveAsAsync(SaveParameters saveParameters)
	{
		Logger.LogWarning("SaveAs is not currently implemented");
		return Task.FromResult<string?>(null);
	}

	//public override void Save()
	//	=> Save(CurrentFile.Filename);

	//public override void SaveAs(SaveParameters saveParameters)
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
	//	logger?.Info("Saving scenario/save/landscape to {Filename}", filename);

	//	var newFile = CurrentS5File with
	//	{
	//		RequiredObjects = [.. RequiredObjects.Select(x => x.GetAsUnderlyingType())],
	//	};

	//	var bytes = newFile.Write();
	//	File.WriteAllBytes(filename, bytes);
	//}
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types.SCV5;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Gui.Models;
using Index;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
	public ReactiveCommand<Unit, Unit> ValidateSCV5Command { get; }

	public SCV5ViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
	{
		RequiredObjects = new RequiredObjectsListViewModel(editorContext);
		Load();
		DownloadMissingObjectsToGameObjDataCommand = ReactiveCommand.CreateFromTask<GameObjDataFolder>(DownloadMissingObjects);
		ValidateSCV5Command = ReactiveCommand.CreateFromTask(ValidateSCV5Async);
	}

	public override void Load()
	{
		Logger.LogInformation("Loading scenario/save from {FileName}", CurrentFile.FileName);
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

		var po = Model.PackedObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x.Header.Convert())).OrderBy(x => x.Name);
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

	async Task ValidateSCV5Async()
	{
		if (Model == null)
		{
			Logger.LogError("Cannot validate scenario because the model is null");
			return;
		}

		if (EditorContext.ObjectIndex.Objects.Count == 0)
		{
			var infoBox = MessageBoxManager.GetMessageBoxStandard(
				"Scenario validation",
				"No object index is loaded. Load an ObjData directory first so the scenario's objects can be resolved.",
				ButtonEnum.Ok,
				Icon.Info,
				windowStartupLocation: WindowStartupLocation.CenterOwner);

			_ = infoBox.ShowAsync();
			return;
		}

		// The scenario's required objects, converted to ObjectModelHeaders (skip empty/fill slots).
		var scenarioObjects = Model.RequiredObjects
			.Where(x => x.Checksum != 0)
			.Select(x => x.Convert())
			.ToList();

		// Loading object files to resolve dependencies can be slow, so do it off the UI thread.
		var validationErrors = await Task.Run(() => ObjectValidation.ValidateSCV5(scenarioObjects, ResolveObjectDependencies));

		await ShowValidationMessageBox(validationErrors, showPopupOnSuccess: true);
	}

	/// <summary>
	/// Resolves the object headers that a scenario-included object depends on being present. For
	/// industries this is the cargo they produce and consume. Objects that cannot be resolved (e.g.
	/// not present in the loaded ObjData index) are treated as having no dependencies.
	/// </summary>
	IEnumerable<ObjectModelHeader> ResolveObjectDependencies(ObjectModelHeader header)
	{
		var entry = EditorContext.ObjectIndex.Objects
			.FirstOrDefault(x => x.DisplayName == header.Name && x.DatChecksum == header.DatChecksum);

		if (entry?.FileName == null)
		{
			return [];
		}

		var path = Path.Combine(EditorContext.Settings.ObjDataDirectory, entry.FileName);
		if (!File.Exists(path))
		{
			return [];
		}

		var (_, locoObject) = SawyerStreamReader.LoadFullObject(path, Logger, loadExtra: false);
		return ObjectValidation.GetObjectDependencies(locoObject?.Object);
	}

	static async Task ShowValidationMessageBox(IEnumerable<string> validationErrors, bool showPopupOnSuccess)
	{
		// Show the box as a modal dialog owned by the main window so the user must dismiss it
		// before interacting with the editor again.
		var owner = Application.Current?.ApplicationLifetime switch
		{
			IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
			_ => null,
		};

		if (validationErrors.Any())
		{
			var errorMsg = string.Join(Environment.NewLine, validationErrors);
			var box = MessageBoxManager.GetMessageBoxStandard(
				new MessageBoxStandardParams
				{
					ContentTitle = "Validation failed",
					ContentMessage = errorMsg,
					ButtonDefinitions = ButtonEnum.Ok,
					Icon = Icon.Error,
					WindowStartupLocation = WindowStartupLocation.CenterOwner,
					Topmost = true,
				});

			_ = owner == null ? box.ShowAsync() : box.ShowWindowDialogAsync(owner);
		}
		else
		{
			if (showPopupOnSuccess)
			{
				var box = MessageBoxManager.GetMessageBoxStandard(
					new MessageBoxStandardParams
					{
						ContentTitle = "Validation succeeded",
						ContentMessage = "✔ No issues found. SCV5 file is valid.",
						ButtonDefinitions = ButtonEnum.Ok,
						Icon = Icon.Success,
						WindowStartupLocation = WindowStartupLocation.CenterOwner,
						Topmost = true,
					});

				_ = owner == null ? box.ShowAsync() : box.ShowWindowDialogAsync(owner);
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

		if (EditorContext.ObjectServiceClient == null)
		{
			Logger.LogError("The object service client is null");
			return;
		}

		var gameFolderIndex = await ObjectIndex.LoadOrCreateIndexAsync(folder, Logger).ConfigureAwait(true);

		if (EditorContext.ObjectIndexOnline == null)
		{
			// need to download the index, ie call /objects/list
			Logger.LogInformation("Online index doesn't exist - downloading now");

			EditorContext.ObjectIndexOnline = new ObjectIndex((await EditorContext.ObjectServiceClient.GetObjectListAsync())
				.Select(x => new ObjectIndexEntry(x.DisplayName, null, x.Id, x.DatChecksum, null, x.ObjectType, x.ObjectSource, x.CreatedDate, x.ModifiedDate, x.VehicleType)));

			Logger.LogInformation("Index downloaded");
			// technically should check if the index is downloaded and valid now
		}

		// Download the scenario's required objects and also any of their dependency objects that
		// are missing from the target folder.
		var objectIndexOnline = EditorContext.ObjectIndexOnline!; // guaranteed non-null above
		foreach (var obj in GetRequiredObjectsWithDependencies())
		{
			await DownloadMissingObjectAsync(obj, folder, gameFolderIndex, objectIndexOnline);
		}
	}

	/// <summary>
	/// Collects every object that should be present in the game folder for this scenario to run:
	/// the scenario's required objects, plus each one's dependency objects (resolved by reusing the
	/// dependency-checking code). Duplicate objects are removed so each is only downloaded once.
	/// </summary>
	List<ObjectModelHeader> GetRequiredObjectsWithDependencies()
	{
		var desired = new List<ObjectModelHeader>();
		var seen = new HashSet<(string Name, uint Checksum)>();

		foreach (var required in RequiredObjects.Items)
		{
			if (required == null || string.IsNullOrWhiteSpace(required.Name) || required.DatChecksum == 0)
			{
				continue;
			}

			if (seen.Add((required.Name, required.DatChecksum)))
			{
				desired.Add(required);
			}

			// Reuse the dependency-checking code to discover this object's dependencies
			// (e.g. an industry's produced/consumed cargo, a track's tunnels/stations, etc.).
			foreach (var dependency in ResolveObjectDependencies(required))
			{
				if (dependency == null || string.IsNullOrWhiteSpace(dependency.Name) || dependency.DatChecksum == 0)
				{
					continue;
				}

				if (seen.Add((dependency.Name, dependency.DatChecksum)))
				{
					desired.Add(dependency);
				}
			}
		}

		return desired;
	}

	/// <summary>
	/// Downloads a single missing object into <paramref name="folder"/>. Vanilla objects and objects
	/// already present in the game folder are skipped.
	/// </summary>
	async Task DownloadMissingObjectAsync(ObjectModelHeader obj, string folder, ObjectIndex gameFolderIndex, ObjectIndex objectIndexOnline)
	{
		if (obj == null || string.IsNullOrWhiteSpace(obj.Name) || obj.DatChecksum == 0)
		{
			return;
		}

		if (OriginalObjectFiles.GetFileSource(obj.Name, obj.DatChecksum, obj.ObjectSource.Convert()) is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
		{
			return;
		}

		if (gameFolderIndex.Objects.Contains(x => x.DisplayName == obj.Name && x.DatChecksum == obj.DatChecksum))
		{
			return;
		}

		// obj is missing - we need to download
		Logger.LogInformation("Scenario {DisplayName} has missing object. Name=\"{Name}\" Checksum={DatChecksum} ObjectType={ObjectType} ", CurrentFile.DisplayName, obj.Name, obj.DatChecksum, obj.ObjectType);

		var onlineObj = objectIndexOnline
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

			var result = await EditorContext.ObjectServiceClient.AddMissingObjectAsync(missingEntry);
			if (result != null)
			{
				Logger.LogInformation("Successfully added missing object to server: Id={Id} Name=\"{Name}\" Checksum=({DatChecksum})", result.Id, obj.Name, obj.DatChecksum);
			}
			else
			{
				Logger.LogError("Failed to add missing object to server: Name=\"{Name}\" Checksum=({DatChecksum})", obj.Name, obj.DatChecksum);
			}

			return;
		}

		if (onlineObj.Id == null)
		{
			Logger.LogError("Downloaded object had no Id - this is a problem with the server");
			return;
		}

		// download actual file
		var downloadedObjBytes = await EditorContext.ObjectServiceClient.GetObjectFileAsync(onlineObj.Id.Value);

		if (downloadedObjBytes == null)
		{
			Logger.LogError("Downloaded bytes was null");
			return;
		}

		// write file to the selected directory
		var filename = $"{onlineObj.DisplayName ?? onlineObj.FileName}-{onlineObj.Id}.dat";
		filename = Path.Combine(folder, filename);

		if (File.Exists(filename))
		{
			Logger.LogWarning("{Filename} already exists - will NOT overwrite it", filename);
			return;
		}

		Logger.LogInformation("Writing file to {Filename}", filename);

		await File.WriteAllBytesAsync(filename, downloadedObjBytes);
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
	{
		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? CurrentFile.FileName
			: Path.Combine(EditorContext.Settings.DownloadFolder, Path.ChangeExtension($"{CurrentFile.DisplayName}-{CurrentFile.Id}", ".sv5"));

		if (string.IsNullOrEmpty(savePath))
		{
			Logger.LogError("Cannot save scenario/save file because save path is empty");
			return;
		}

		SaveCore(savePath);
	}

	public override async Task<string?> SaveAsAsync(SaveParameters saveParameters)
	{
		var saveFile = await PlatformSpecific.SaveFilePicker(PlatformSpecific.SCV5FileTypes);
		if (saveFile == null)
		{
			return null;
		}

		var savePath = saveFile.Path.LocalPath;
		SaveCore(savePath);
		return savePath;
	}

	void SaveCore(string filename)
	{
		if (Model == null)
		{
			Logger.LogError("Cannot save scenario/save file because model is null");
			return;
		}

		var newFile = Model with
		{
			RequiredObjects = [.. RequiredObjects.Items.Select(x => x.Convert())],
		};

		var bytes = newFile.Write();
		File.WriteAllBytes(filename, bytes);
		Logger.LogInformation("Saved scenario/save file to {Filename}", filename);
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

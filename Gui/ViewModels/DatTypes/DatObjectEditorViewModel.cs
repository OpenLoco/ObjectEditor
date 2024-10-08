using AvaGui.Models;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Objects.Sound;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaGui.ViewModels
{
	public class DatObjectEditorViewModel : BaseLocoFileViewModel
	{
		ObjectEditorModel Model { get; init; }
		ILogger? Logger => Model.Logger;

		[Reactive]
		public StringTableViewModel? StringTableViewModel { get; set; }

		[Reactive]
		public IExtraContentViewModel? ExtraContentViewModel { get; set; }

		[Reactive]
		public VehicleViewModel? VehicleVM { get; set; }

		[Reactive]
		public UiDatLocoFile? CurrentObject { get; private set; }

		public IObjectViewModel CurrentObjectViewModel
			=> VehicleVM == null
				? new GenericObjectViewModel() { Object = CurrentObject.LocoObject.Object }
				: VehicleVM;

		[Reactive]
		public ObservableCollection<TreeNode> CurrentHexAnnotations { get; private set; }

		[Reactive]
		public TreeNode? CurrentlySelectedHexAnnotation { get; set; }

		[Reactive]
		public HexAnnotationLine[]? CurrentHexDumpLines { get; set; }

		byte[] currentByteList;

		Dictionary<string, (int Start, int End)> DATDumpAnnotationIdentifiers = [];
		const int bytesPerDumpLine = 32;
		const int addressStringSizeBytes = 8;
		const int addressStringSizePrependBytes = addressStringSizeBytes + 2;
		const int dumpWordSize = 4;

		public DatObjectEditorViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		{
			CurrentFile = currentFile;
			Model = model;

			LoadObject();

			ReloadCommand = ReactiveCommand.Create(LoadObject);
			SaveCommand = ReactiveCommand.Create(SaveCurrentObject);
			SaveAsCommand = ReactiveCommand.Create(SaveAsCurrentObject);

			_ = this.WhenAnyValue(o => o.CurrentlySelectedHexAnnotation)
				.Subscribe(_ => UpdateHexDumpView());
		}

		public void UpdateHexDumpView()
		{
			if (CurrentlySelectedHexAnnotation != null && DATDumpAnnotationIdentifiers.TryGetValue(CurrentlySelectedHexAnnotation.Title, out var positionValues))
			{
				CurrentHexDumpLines = GetDumpLines(currentByteList, positionValues.Start, positionValues.End).ToArray();
			}
			else
			{
				CurrentHexDumpLines = GetDumpLines(currentByteList, null, null).ToArray();
			}
		}

		public void LoadObject()
		{
			// this stops any currently-playing sounds
			if (ExtraContentViewModel is SoundViewModel svm)
			{
				svm.Dispose();
			}

			Logger?.Info($"Loading {CurrentFile.DisplayName} from {CurrentFile.Filename}");

			if (Model.TryLoadObject(CurrentFile, out var newObj))
			{
				CurrentObject = newObj;

				if (CurrentObject?.LocoObject != null)
				{
					if (CurrentObject.LocoObject.Object is VehicleObject veh)
					{
						VehicleVM = new VehicleViewModel()
						{
							Mode = veh.Mode,
							Type = veh.Type,
							var_04 = veh.var_04,
							TrackTypeId = veh.TrackTypeId,
							CostIndex = veh.CostIndex,
							CostFactor = veh.CostFactor,
							Reliability = veh.Reliability,
							RunCostIndex = veh.RunCostIndex,
							RunCostFactor = veh.RunCostFactor,
							ColourType = veh.ColourType,
							CompatibleVehicles = new(veh.CompatibleVehicles),
							RequiredTrackExtras = new(veh.RequiredTrackExtras),
							CarComponents = new(veh.CarComponents),
							BodySprites = new(veh.BodySprites),
							BogieSprites = new(veh.BogieSprites),
							Power = veh.Power,
							Speed = veh.Speed,
							RackSpeed = veh.RackSpeed,
							Weight = veh.Weight,
							Flags = veh.Flags,
							MaxCargo = new(veh.MaxCargo),
							CompatibleCargoCategories1 = new(veh.CompatibleCargoCategories[0]),
							CompatibleCargoCategories2 = new(veh.CompatibleCargoCategories[1]),
							CargoTypeSpriteOffsets = new(veh.CargoTypeSpriteOffsets.Select(x => new CargoTypeSpriteOffset(x.Key, x.Value)).ToList()),
							Animation = new(veh.Animation),
							AnimationHeaders = new(veh.AnimationHeaders),
							var_113 = veh.var_113,
							DesignedYear = veh.Designed,
							ObsoleteYear = veh.Obsolete,
							RackRailType = veh.RackRailType,
							SoundType = veh.SoundType,
							StartSounds = new(veh.StartSounds),
							FrictionSound = veh.SoundPropertyFriction,
							Engine1Sound = veh.SoundPropertyEngine1,
							Engine2Sound = veh.SoundPropertyEngine2,
						};
					}
					else
					{
						VehicleVM = null;
					}

					var imageNameProvider = (CurrentObject.LocoObject.Object is IImageTableNameProvider itnp) ? itnp : new DefaultImageTableNameProvider();
					StringTableViewModel = new(CurrentObject.LocoObject.StringTable);
					ExtraContentViewModel = CurrentObject.LocoObject.Object is SoundObject
						? new SoundViewModel(CurrentObject.LocoObject)
						: new ImageTableViewModel(CurrentObject.LocoObject, imageNameProvider, Model.PaletteMap, CurrentObject.Images, Model.Logger);

					var (treeView, annotationIdentifiers) = AnnotateFile(Path.Combine(Model.Settings.ObjDataDirectory, CurrentFile.Filename), false, null);
					CurrentHexAnnotations = new(treeView);
					DATDumpAnnotationIdentifiers = annotationIdentifiers;
				}
				else
				{
					StringTableViewModel = null;
					ExtraContentViewModel = null;
				}
			}
			else
			{
				// todo: show warnings here
				CurrentObject = null;
			}
		}

		public void SaveCurrentObject()
		{
			if (CurrentObject?.LocoObject == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			var savePath = CurrentFile.FileLocation == FileLocation.Local
				? Path.Combine(Model.Settings.ObjDataDirectory, CurrentFile.Filename)
				: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {savePath}");
			StringTableViewModel?.WriteTableBackToObject();
			if (VehicleVM != null && CurrentObject.LocoObject.Object is VehicleObject veh)
			{
				// convert VehicleVM back to DAT, eg cargo sprites, string table, etc
				// this can probably go in VehicleViewModelClass
				foreach (var ctso in VehicleVM.CargoTypeSpriteOffsets)
				{
					veh.CargoTypeSpriteOffsets[ctso.CargoCategory] = ctso.Offset;
				}

				CurrentObject.LocoObject.Object = veh with
				{
					NumCompatibleVehicles = (byte)VehicleVM.CompatibleVehicles.Count,
					NumRequiredTrackExtras = (byte)VehicleVM.RequiredTrackExtras.Count,
					NumStartSounds = (byte)VehicleVM.StartSounds.Count,
				};
			}

			SawyerStreamWriter.Save(savePath, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject, Logger);
		}

		public void SaveAsCurrentObject()
		{
			var saveFile = Task.Run(PlatformSpecific.SaveFilePicker).Result;
			if (saveFile == null)
			{
				return;
			}

			if (CurrentObject?.LocoObject == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			var savePath = saveFile.Path.LocalPath;
			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {savePath}");
			StringTableViewModel?.WriteTableBackToObject();

			SawyerStreamWriter.Save(savePath, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject, Logger);
		}

		(IList<TreeNode> treeView, Dictionary<string, (int, int)> annotationIdentifiers) AnnotateFile(string path, bool isG1 = false, ILogger? logger = null)
		{
			if (!File.Exists(path))
			{
				return ([], []);
			}

			IList<HexAnnotation> annotations = [];
			currentByteList = File.ReadAllBytes(path);

			try
			{
				annotations = isG1
					? ObjectAnnotator.AnnotateG1Data(currentByteList)
					: ObjectAnnotator.Annotate(currentByteList);

				return AnnotateFileCore(annotations);
			}
			catch (Exception ex)
			{
				logger?.Error(ex);
				return ([], []);
			}
		}

		static (IList<TreeNode> treeView, Dictionary<string, (int, int)> annotationIdentifiers) AnnotateFileCore(IList<HexAnnotation> annotations)
		{
			var treeView = new List<TreeNode>();
			var parents = new Dictionary<string, TreeNode>();
			Dictionary<string, TreeNode> imageHeaderIndexToNode = [];
			Dictionary<string, TreeNode> imageDataIndexToNode = [];
			Dictionary<string, (int, int)> datDumpAnnotationIdentifiers = [];

			foreach (var annotation in annotations)
			{
				var annotationText = annotation.Name;
				parents[annotationText] = new TreeNode(annotation.Name, annotation.OffsetText);
				datDumpAnnotationIdentifiers[annotationText] = (annotation.Start, annotation.End);
				if (annotation.Parent == null)
				{
					treeView.Add(parents[annotation.Name]);
				}
				else if (parents.TryGetValue(annotation.Parent.Name, out var parentNode))
				{
					parentNode.Nodes.Add(parents[annotationText]);

					if (annotation.Parent.Name == "Headers")
					{
						imageHeaderIndexToNode[annotation.Name] = parents[annotationText];
					}

					if (annotation.Parent.Name == "Images")
					{
						imageDataIndexToNode[annotation.Name] = parents[annotationText];
					}
				}
			}

			return (treeView, datDumpAnnotationIdentifiers);
		}

		private static IEnumerable<HexAnnotationLine> GetDumpLines(byte[] byteList, int? selectionStart, int? selectionEnd)
		{
			if (byteList == null)
			{
				yield break;
			}

			var count = 0;

			foreach (var b in byteList.Chunk(bytesPerDumpLine))
			{
				int? sStart = selectionStart != null && (selectionStart >= count || selectionEnd > count)
					? (Math.Max(selectionStart.Value, count) * 2) - (count * 2) : null;

				int? sEnd = selectionEnd != null && sStart != null && selectionEnd >= count
					? (Math.Min(selectionEnd.Value, count + bytesPerDumpLine) * 2) - (count * 2) : null;

				yield return new HexAnnotationLine(
					string.Format("{0:X" + addressStringSizeBytes + "}", count),
					string.Join(" ", b.Chunk(4).Select(x => string.Concat(x.Select(y => y.ToString("X2"))))),
					sStart + (sStart / 8),
					sEnd + (sEnd / 8));

				count += bytesPerDumpLine;
			}
		}
	}

	public class TreeNode(string title, string offsetText, ObservableCollection<TreeNode> nodes)
	{
		public ObservableCollection<TreeNode>? Nodes { get; } = nodes;
		public string Title { get; } = title;
		public string OffsetText { get; } = offsetText;
		public TreeNode() : this("<empty>", "<empty>") { }

		public TreeNode(string title, string offsetText) : this(title, offsetText, []) { }
	}
}
